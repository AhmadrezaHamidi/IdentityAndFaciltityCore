using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FacilityService.Api;

/// <summary>
/// همهٔ خطاهای API پنل Facility با یک قالب برمی‌گردند (همان قالب FacilityBaseMessageResponse):
///   { "statusCode": 400, "message": "پیام فارسی", "errors": { "field": ["..."] } }
/// - خطای اعتبارسنجی ورودی (ModelState) ← 400 با پیام فارسی و فهرست فیلدها (به‌جای ProblemDetails انگلیسی)
/// - خطای پیش‌بینی‌نشده ← 500 با پیام عمومی فارسی؛ جزئیات فقط در لاگ (و در محیط توسعه در فیلد detail)
/// Program.cs:  builder.Services.AddFacilityPanelErrorHandling();  و  app.UseFacilityPanelErrorHandling();
/// </summary>
public static class FacilityPanelErrorHandling
{
    public const string InvalidInputMessage = "اطلاعات ورودی معتبر نیست.";
    public const string UnexpectedMessage = "خطای پیش‌بینی‌نشده‌ای رخ داد؛ دوباره تلاش کنید و اگر تکرار شد به پشتیبانی اطلاع دهید.";

    public static IServiceCollection AddFacilityPanelErrorHandling(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value is { Errors.Count: > 0 })
                    .ToDictionary(
                        e => JsonNamingPolicy.CamelCase.ConvertName(e.Key.TrimStart('$', '.')),
                        e => e.Value!.Errors.Select(x => Translate(x.ErrorMessage)).Distinct().ToArray());
                var first = errors.Values.SelectMany(x => x).FirstOrDefault();
                var message = first is null ? InvalidInputMessage : $"{InvalidInputMessage} {first}";
                return new BadRequestObjectResult(new { statusCode = StatusCodes.Status400BadRequest, message, errors });
            };
        });
        return services;
    }

    public static IApplicationBuilder UseFacilityPanelErrorHandling(this IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        return app.UseExceptionHandler(builder => builder.Run(async context =>
        {
            var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("FacilityPanel");
            logger.LogError(error, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);

            var (status, message) = error switch
            {
                BadHttpRequestException { StatusCode: StatusCodes.Status413PayloadTooLarge } =>
                    (StatusCodes.Status413PayloadTooLarge, "حجم فایل یا درخواست بیش از حد مجاز است."),
                BadHttpRequestException => (StatusCodes.Status400BadRequest, InvalidInputMessage),
                OperationCanceledException => (499, "درخواست لغو شد."),
                _ => (StatusCodes.Status500InternalServerError, UnexpectedMessage),
            };
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json; charset=utf-8";
            object body = env.IsDevelopment()
                ? new { statusCode = status, message, detail = error?.Message }
                : new { statusCode = status, message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(body, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
        }));
    }

    /// <summary>پیام‌های پیش‌فرض انگلیسی ASP.NET برای ورودی‌های رایج.</summary>
    private static string Translate(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return "مقدار نامعتبر است.";
        if (message.Contains("is required", StringComparison.OrdinalIgnoreCase)) return "این فیلد الزامی است.";
        if (message.Contains("could not be converted", StringComparison.OrdinalIgnoreCase)
            || message.Contains("is not valid", StringComparison.OrdinalIgnoreCase)
            || message.Contains("JSON", StringComparison.Ordinal))
            return "قالب مقدار واردشده درست نیست.";
        if (message.Contains("maximum length", StringComparison.OrdinalIgnoreCase)) return "طول مقدار بیش از حد مجاز است.";
        if (message.Contains("minimum length", StringComparison.OrdinalIgnoreCase)) return "طول مقدار کمتر از حد مجاز است.";
        if (message.Contains("must be between", StringComparison.OrdinalIgnoreCase)) return "مقدار خارج از بازهٔ مجاز است.";
        return message;   // پیام‌های فارسی که خود قراردادها تعریف کرده‌اند بدون تغییر برمی‌گردند
    }
}
