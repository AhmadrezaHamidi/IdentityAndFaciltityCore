using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FacilityService.Api.Swagger;

/// <summary>
/// [JsonIgnore] فقط بدنهٔ JSON را پوشش می‌دهد؛ در مدل‌های [FromQuery] خاصیت‌هایی مثل Kind یا
/// RequestInfo.* (از BaseQuery) باز هم به‌عنوان پارامتر QueryString در Swagger می‌آیند.
/// این فیلتر هر پارامتری را که خاصیتش (یا خاصیت والدش) [JsonIgnore] یا [BindNever] دارد حذف می‌کند.
/// </summary>
public sealed class HideIgnoredParametersFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        HideIgnoredFormFields(operation, context);
        if (operation.Parameters is null || operation.Parameters.Count == 0) return;

        var hidden = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in context.ApiDescription.ParameterDescriptions)
        {
            if (p.Source != BindingSource.Query || p.ModelMetadata?.ContainerType is null) continue;

            // نام کامل مثل «RequestInfo.Token»: اگر هر بخش مسیر Ignore باشد، حذف می‌شود
            var type = p.ModelMetadata.ContainerType;
            var segments = p.Name.Split('.');
            var root = context.ApiDescription.ParameterDescriptions
                .FirstOrDefault(x => x.Name == p.Name)?.ParameterDescriptor?.ParameterType;

            if (IsIgnored(root, segments) || IsIgnored(type, [segments[^1]]))
                hidden.Add(p.Name);
        }

        foreach (var parameter in operation.Parameters.Where(x => hidden.Contains(x.Name)).ToList())
            operation.Parameters.Remove(parameter);
    }

    /// <summary>همین قاعده برای بدنه‌های multipart/form-data ([FromForm]): Id مسیر و فایل‌هایی که کنترلر پر می‌کند.</summary>
    private static void HideIgnoredFormFields(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody?.Content is null) return;

        // نام خاصیت‌های Ignore در مدل‌های [FromForm] اکشن (مستقیم از نوع پارامتر، مستقل از نام‌گذاری ApiExplorer)
        var ignored = context.MethodInfo.GetParameters()
            .Where(p => p.IsDefined(typeof(Microsoft.AspNetCore.Mvc.FromFormAttribute), false))
            .SelectMany(p => p.ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            .Where(prop => prop.IsDefined(typeof(JsonIgnoreAttribute), true) || prop.IsDefined(typeof(BindNeverAttribute), true))
            .Select(prop => prop.Name)
            .ToList();
        if (ignored.Count == 0) return;

        foreach (var media in operation.RequestBody.Content.Values)
        {
            if (media.Schema?.Properties is null) continue;
            var remove = media.Schema.Properties.Keys
                .Where(k => ignored.Any(i => string.Equals(k, i, StringComparison.Ordinal) ||
                                             k.StartsWith(i + ".", StringComparison.Ordinal)))
                .ToList();
            foreach (var key in remove)
            {
                media.Schema.Properties.Remove(key);
                media.Encoding?.Remove(key);
            }
        }
    }

    private static bool IsIgnored(Type? type, string[] path)
    {
        foreach (var segment in path)
        {
            if (type is null) return false;
            var prop = type.GetProperty(segment, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop is null) return false;
            if (prop.IsDefined(typeof(JsonIgnoreAttribute), true) || prop.IsDefined(typeof(BindNeverAttribute), true))
                return true;
            type = prop.PropertyType;
        }
        return false;
    }
}
