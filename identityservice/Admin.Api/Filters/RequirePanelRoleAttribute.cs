using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OmidBank.Common.Interface;

namespace Admin.Api.Filters;


[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class RequirePanelRoleAttribute : ActionFilterAttribute
{
    private readonly string[] _allowedRoles;

    public RequirePanelRoleAttribute(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var handlerUtility = context.HttpContext.RequestServices.GetService<IAdminHandlerUtility>();
        var jwtSettings = context.HttpContext.RequestServices.GetService<IOptions<JwtSettings>>()?.Value;

        var token = handlerUtility?.ReadCookie("Authorization");
        if (string.IsNullOrWhiteSpace(token) || jwtSettings is null)
        {
            Deny(context);
            return;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.JwtSecretKey)),
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                ClockSkew = new TimeSpan(0, jwtSettings.ExpirationMinutes, 0),
            }, out var validatedToken);

            var roleNamesJson = ((JwtSecurityToken)validatedToken).Claims
                .FirstOrDefault(c => c.Type == "RoleNames")?.Value;

            var roleNames = string.IsNullOrWhiteSpace(roleNamesJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(roleNamesJson) ?? new List<string>();

            if (!roleNames.Any(r => _allowedRoles.Contains(r, StringComparer.OrdinalIgnoreCase)))
            {
                Deny(context);
            }
        }
        catch
        {
            Deny(context);
        }
    }

    private static void Deny(ActionExecutingContext context)
    {
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = 403;
        context.Result = new Microsoft.AspNetCore.Mvc.ContentResult
        {
            StatusCode = 403,
            Content = "{}",
            ContentType = "application/json",
        };
    }
}