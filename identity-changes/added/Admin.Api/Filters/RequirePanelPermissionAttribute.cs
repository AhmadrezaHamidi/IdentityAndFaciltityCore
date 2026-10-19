using IdentityService.Domain.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace Admin.Api.Filters;

/// <summary>
/// جایگزین <c>RequirePanelRoleAttribute</c>.
///
/// تفاوت بنیادی: آن یکی نام نقش را از claim توکن می‌خواند، پس وقتی ادمین مجوزهای یک نقش را
/// عوض می‌کرد هیچ اثری روی endpointها نداشت — خلاف «کلیه دسترسی به کارکردها و سرویس‌های هر
/// بخش توسط ادمین قابل تغییر است». این یکی کلید مجوز را می‌خواهد و مجوزها را لحظه‌ای از
/// دیتابیس می‌گیرد، پس تغییر ادمین فوراً اثر می‌کند.
///
/// روی کنترلر یا اکشن. اگر هر دو داشته باشند، اکشن برنده است (نه جمع هر دو).
/// چک دقیق روی منبع در Handler با <c>permissions.Can(key, resource)</c> انجام می‌شود —
/// این فیلتر فقط گیت درشت است.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class RequirePanelPermissionAttribute(params string[] anyOfKeys) : Attribute
{
    public string[] AnyOfKeys { get; } = anyOfKeys;
}

/// <summary>این endpoint توکن کاربر پنل نمی‌خواهد (ورود، کپچا، health).</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class PanelAnonymousAttribute : Attribute;

/// <summary>
/// یک فیلتر Global برای همهٔ اکشن‌ها. در Program.cs:
///     builder.Services.Configure&lt;MvcOptions&gt;(o =&gt; o.Filters.Add&lt;PanelPermissionFilter&gt;());
///
/// اکشنی که نه <see cref="RequirePanelPermissionAttribute"/> دارد و نه
/// <see cref="PanelAnonymousAttribute"/>، بسته است و در لاگ به‌عنوان خطای پیکربندی ثبت می‌شود.
/// علت: در مدل قبلی نبودن attribute یعنی endpoint باز، و فراموش کردنش یک رخنهٔ بی‌صدا می‌ساخت.
/// </summary>
public sealed class PanelPermissionFilter(
    IPanelSessionResolver sessions,
    ILogger<PanelPermissionFilter> logger) : IAsyncAuthorizationFilter
{
    /// <summary>کلیدی که کاربر جاری زیر آن در HttpContext.Items گذاشته می‌شود.</summary>
    public const string CurrentUserKey = "__PanelCurrentUser";

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor action)
        {
            logger.LogError("Panel permission filter ran on a non-controller endpoint: {Path}", context.HttpContext.Request.Path);
            context.Result = Error(StatusCodes.Status403Forbidden, "به این بخش دسترسی ندارید.");
            return;
        }

        if (Has<PanelAnonymousAttribute>(action))
            return;

        var required = Required(action);
        if (required is null)
        {
            logger.LogError(
                "No [RequirePanelPermission] on {Controller}.{Action} — denied. یا کلید را اعلام کن یا [PanelAnonymous] بگذار.",
                action.ControllerName, action.ActionName);
            context.Result = Error(StatusCodes.Status403Forbidden, "به این بخش دسترسی ندارید.");
            return;
        }

        var session = await sessions.ResolveAsync(context.HttpContext.RequestAborted);

        // نشست باطل: توکن نامعتبر، کاربر غیرفعال، یا دسترسی عوض شده (معیار ۶) ← ۴۰۱ و بازگشت به صفحهٔ ورود.
        if (session is null)
        {
            context.Result = Error(StatusCodes.Status401Unauthorized, "نشست نامعتبر است؛ دوباره وارد شوید.");
            return;
        }

        if (required.Length > 0 && !session.Permissions.HasAnyOf(required))
        {
            logger.LogWarning("Panel permission denied. User {UserId} lacks any of [{Keys}] for {Path}",
                session.PanelUserId, string.Join(", ", required), context.HttpContext.Request.Path);
            context.Result = Error(StatusCodes.Status403Forbidden, "به این بخش دسترسی ندارید.");
            return;
        }

        context.HttpContext.Items[CurrentUserKey] = session;
    }

    private static string[]? Required(ControllerActionDescriptor action) =>
        action.MethodInfo.GetCustomAttribute<RequirePanelPermissionAttribute>(true)?.AnyOfKeys
        ?? action.ControllerTypeInfo.GetCustomAttribute<RequirePanelPermissionAttribute>(true)?.AnyOfKeys;

    private static bool Has<T>(ControllerActionDescriptor action) where T : Attribute =>
        action.MethodInfo.GetCustomAttribute<T>(true) is not null
        || action.ControllerTypeInfo.GetCustomAttribute<T>(true) is not null;

    private static ObjectResult Error(int status, string message) =>
        new(new { statusCode = status, message }) { StatusCode = status };
}
