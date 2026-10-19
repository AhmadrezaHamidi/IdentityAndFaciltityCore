using FacilityService.Api.Authorization;
using FacilityService.Application.Authorization;

namespace FacilityService.Api;

/// <summary>
/// احراز مجوز پنل در Facility. Program.cs:  builder.Services.AddFacilityPanelAuthorization(builder.Configuration);
/// تنظیمات: Identity:BaseUrl (آدرس Admin.Api/IdentityService) و InternalApi:Key (همان کلید دو طرف).
/// </summary>
public static class FacilityPanelServiceRegistration
{
    public static IServiceCollection AddFacilityPanelAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddScoped<IFacilityCurrentUser, FacilityCurrentUserAccessor>();
        services.AddHttpClient<IIdentityAuthorizationClient, IdentityAuthorizationClient>(client =>
        {
            var baseUrl = configuration["Identity:BaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl)) client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            var key = configuration["InternalApi:Key"];
            if (!string.IsNullOrWhiteSpace(key)) client.DefaultRequestHeaders.Add("X-Api-Key", key);
            client.Timeout = TimeSpan.FromSeconds(5);
        });
        return services;
    }
}
