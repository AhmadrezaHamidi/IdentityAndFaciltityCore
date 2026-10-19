using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FacilityService.Api.Attributes;

/// <summary>Endpointهای داخلی که Identity صدا می‌زند: هدر X-Api-Key = تنظیم «InternalApi:Key». بدون تنظیم بسته است.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class FacilityInternalApiKeyAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var expected = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["InternalApi:Key"];
        var sent = context.HttpContext.Request.Headers["X-Api-Key"].ToString();
        if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(sent) ||
            !CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(sent)))
            context.Result = new UnauthorizedResult();
    }
}
