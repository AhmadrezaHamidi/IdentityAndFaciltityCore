using System.Reflection;
using FacilityService.Api.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FacilityService.Api.Swagger;

/// <summary>
/// Swagger پنل Facility: توضیح کوتاه بالای صفحه + راهنمای کامل Flowها و Endpointها روی /guide (guide.html، EmbeddedResource؛ tools/build-api-guide.py).
/// Program.cs:  builder.Services.AddFacilitySwagger();  …  app.UseFacilitySwagger();
/// </summary>
public static class FacilitySwaggerSetup
{
    public const string DocumentName = "v2";

    public static IServiceCollection AddFacilitySwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(DocumentName, new OpenApiInfo
            {
                Title = "Facility Panel API — تسهیلات سازمانی (نگین میثاق)",
                Version = DocumentName,
                Description = Summary,
            });
            c.EnableAnnotations();
            c.SupportNonNullableReferenceTypes();
            c.CustomSchemaIds(SchemaId);
            c.OperationFilter<HideIgnoredParametersFilter>();
            c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });

            c.AddSecurityDefinition("cookie", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Cookie,
                Name = FacilityPermissionFilter.TokenCookie,
                Description = "همان کوکی ورود پنل (Admin.Api → Authentication/Login).",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "cookie" } }] = [],
            });

            foreach (var xml in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
                if (File.Exists(Path.ChangeExtension(xml, ".dll")))
                    c.IncludeXmlComments(xml, includeControllerXmlComments: true);
        });
        return services;
    }

    public static WebApplication UseFacilitySwagger(this WebApplication app)
    {
        app.MapGet("/guide", () => Results.Content(LoadGuideHtml(), "text/html; charset=utf-8")).ExcludeFromDescription();
        app.UseSwagger();
        app.UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint($"/swagger/{DocumentName}/swagger.json", "Facility Panel v2");
            o.DocumentTitle = "Facility Panel API";
            o.DisplayRequestDuration();
            o.DisplayOperationId();
            o.EnableFilter();
            o.DefaultModelsExpandDepth(0);
        });
        return app;
    }

    private static string SchemaId(Type type)
    {
        static string Name(Type t) => t.IsGenericType
            ? $"{t.Name[..t.Name.IndexOf('`')]}Of{string.Join("And", t.GetGenericArguments().Select(Name))}"
            : t.Name;
        var ns = type.Namespace ?? string.Empty;
        var area = ns.Contains(".Contracts.") ? ns[(ns.LastIndexOf('.') + 1)..] : string.Empty;
        return string.IsNullOrEmpty(area) ? Name(type) : $"{area}.{Name(type)}";
    }

    /// <summary>توضیح کوتاه بالای Swagger؛ شرح کامل Flowها در /guide است.</summary>
    private const string Summary = """
        تسهیلات سازمانی — طرح نگین میثاق: شرکت‌ها، پرسنل، درخواست وام، تأییدها، داشبورد و گزارش تغییرات.

        **[راهنمای کامل: Flowها، ترتیب صدا زدن و فهرست Endpointها ←](/guide)**

        - ورود و منو از Admin.Api است؛ همان کوکی اینجا هم کار می‌کند.
        - `401` ← صفحهٔ ورود · `403` ← دکمه نباید نمایش داده شود · منبع خارج از دامنه ← `404`.
        """;

    private static string LoadGuideHtml()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var name = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith("guide.html", StringComparison.OrdinalIgnoreCase));
        if (name is null) return "<p>راهنما پیدا نشد (Swagger/guide.html باید EmbeddedResource باشد؛ python3 tools/build-api-guide.py).</p>";
        using var reader = new StreamReader(assembly.GetManifestResourceStream(name)!);
        return reader.ReadToEnd();
    }
}
