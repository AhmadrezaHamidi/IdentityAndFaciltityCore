using FacilityService.Api.Authorization;
using FacilityService.Application.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FacilityService.Api.Attributes;

/// <summary>
/// جایگزین [FacilityAnonymous] + ValidateToken دستی. کاربر باید حداقل یکی از کلیدها را در یک دامنه داشته باشد.
/// چک دقیق روی منبع (شرکت/شعبه) در Handler با user.CanOn(...) و فیلتر لیست‌ها با ScopeFor(...).Apply(...) انجام می‌شود.
/// </summary>
public sealed class RequireFacilityPermissionAttribute : TypeFilterAttribute
{
    public RequireFacilityPermissionAttribute(params string[] anyOfKeys) : base(typeof(FacilityPermissionFilter))
    {
        Arguments = [anyOfKeys];
    }
}

public sealed class FacilityPermissionFilter(
    string[] anyOfKeys,
    IIdentityAuthorizationClient identity,
    IConfiguration configuration,
    ILogger<FacilityPermissionFilter> logger) : IAsyncAuthorizationFilter
{
    /// <summary>
    /// نام پیش‌فرض کوکی توکن. باید با کوکی‌ای که Admin.Api هنگام Login می‌گذارد (CookieKey.Token) یکی باشد؛
    /// اگر فرق دارد در تنظیمات «FacilityPanel:TokenCookieName» بگذارید.
    /// </summary>
    public const string TokenCookie = "token";

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var http = context.HttpContext;
        var token = http.Request.Cookies[configuration["FacilityPanel:TokenCookieName"] ?? TokenCookie];
        if (string.IsNullOrEmpty(token))
        {
            var header = http.Request.Headers.Authorization.ToString();
            if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) token = header[7..].Trim();
        }

        if (string.IsNullOrEmpty(token))
        {
            context.Result = Error(StatusCodes.Status401Unauthorized, "نشست نامعتبر است؛ دوباره وارد شوید.");
            return;
        }

        FacilityPanelUser? user;
        try
        {
            user = await identity.ResolveAsync(token, http.RequestAborted);
        }
        catch (Exception ex) when ((ex is HttpRequestException or TaskCanceledException) && !http.RequestAborted.IsCancellationRequested)
        {
            // سرویس Identity در دسترس نیست: بسته می‌ماند (fail-closed) ولی پیام روشن به کاربر داده می‌شود.
            logger.LogError(ex, "Identity authorization service is unreachable for {Path}", http.Request.Path);
            context.Result = Error(StatusCodes.Status503ServiceUnavailable, "سرویس احراز هویت در دسترس نیست؛ چند لحظه بعد دوباره تلاش کنید.");
            return;
        }
        if (user is null)
        {
            context.Result = Error(StatusCodes.Status401Unauthorized, "نشست نامعتبر است؛ دوباره وارد شوید.");
            return;
        }

        if (anyOfKeys.Length > 0 && !anyOfKeys.Any(user.Permissions.HasAnywhere))
        {
            logger.LogWarning("Facility permission denied. User {UserId} lacks any of [{Keys}] for {Path}",
                user.PanelUserId, string.Join(", ", anyOfKeys), http.Request.Path);
            context.Result = Error(StatusCodes.Status403Forbidden, "به این بخش دسترسی ندارید.");
            return;
        }

        http.Items[FacilityCurrentUserAccessor.HttpContextKey] = user;
    }

    // ⚠ قالب را با FacilityBaseMessageResponse پروژه یکی کن.
    private static ObjectResult Error(int status, string message) =>
        new(new { statusCode = status, message }) { StatusCode = status };
}
