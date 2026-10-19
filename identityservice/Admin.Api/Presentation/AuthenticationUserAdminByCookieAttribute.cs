using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using OmidBank.Common.Cache;
using OmidBank.Common.Cookie;
using System.Security.Claims;
using System.Text.Json;

namespace Admin.Api.Presentation.Attributes;

/// <summary>
/// تایید اعتبار کاربر از طریق Secure Cookie (JWT Token)
/// اگر نامعتبر بود: 401 Unauthorized
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthenticationUserAdminByCookieAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContextAccessor = context.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();
        var jwtService = context.HttpContext.RequestServices.GetRequiredService<IJwtService>();
        var activeTokenRepository = context.HttpContext.RequestServices.GetRequiredService<IActiveTokenRepository>();
        var redisCacheManager = context.HttpContext.RequestServices.GetRequiredService<IRedisCacheManager>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthenticationUserAdminByCookieAttribute>>();

        try
        {
            var token = ReadTokenFromCookie(httpContextAccessor.HttpContext);

            if (string.IsNullOrWhiteSpace(token))
            {
                logger.LogWarning("❌ Token در Cookie یافت نشد");
                SetUnauthorizedResponse(context, "بدون Token دسترسی نداری");
                return;
            }

            if (!IsTokenValidInCache(redisCacheManager, token))
            {
                logger.LogWarning("❌ Token در Redis Cache پیدا نشد (شاید منقضی شده)");
                SetUnauthorizedResponse(context, "توکن منقضی‌شده یا نامعتبر است");
                return;
            }

            //var isActive = await activeTokenRepository.IsTokenActiveAsync(token);
            //if (!isActive)
            //{
            //    logger.LogWarning("❌ Token در DB غیرفعال است (Logout شده یا InvalidClaim)");
            //    SetUnauthorizedResponse(context, "این توکن دیگر معتبر نیست");
            //    return;
            //}

            // Step 5: JWT Token رو Validate و Decode کن
            //var claimsPrincipal = jwtService.ValidateToken(token);

            //if (claimsPrincipal == null || claimsPrincipal.Item3.role == null || !claimsPrincipal.Identity.IsAuthenticated)
            //{
            //    logger.LogWarning("❌ JWT Token Signature یا Claims نامعتبر");
            //    SetUnauthorizedResponse(context, "توکن نامعتبر است");
            //    return;
            //}

            //// Step 6: User رو Authenticate کن
            //httpContextAccessor.HttpContext.User = claimsPrincipal;

            //logger.LogInformation("✅ کاربر موفقیت‌امیز Authenticate شد: {UserId}",
            //    claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ خطا در Authentication: {Message}", ex.Message);
            SetUnauthorizedResponse(context, "خطای احراز هویت");
        }
    }

    /// <summary>
    /// Token رو از Secure Cookie بخون
    /// </summary>
    private static string ReadTokenFromCookie(HttpContext httpContext)
    {
        // روش 1: از Cookies Dictionary
        if (httpContext.Request.Cookies.TryGetValue(CookieKey.Token, out var token))
        {
            return token;
        }

        // روش 2: از Authorization Header (اگر SPA بفرستاد)
        var authHeader = httpContext.Request.Headers.Authorization.ToString();
        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader["Bearer ".Length..];
        }

        return null;
    }

    /// <summary>
    /// چک کن Token در Redis Session Cache هستش
    /// </summary>
    private static bool IsTokenValidInCache(IRedisCacheManager cacheManager, string token)
    {
        var cacheKey = string.Format(CacheKey.TosanUserSessionId, token);
        var cachedValue = cacheManager.Read(cacheKey);
        return cachedValue != null;
    }

    /// <summary>
    /// 401 Unauthorized Response برگردون
    /// </summary>
    private static void SetUnauthorizedResponse(AuthorizationFilterContext context, string message)
    {
        context.Result = new UnauthorizedObjectResult(new
        {
            statusCode = 401,
            message = message,
            timestamp = DateTime.UtcNow
        });
    }
}