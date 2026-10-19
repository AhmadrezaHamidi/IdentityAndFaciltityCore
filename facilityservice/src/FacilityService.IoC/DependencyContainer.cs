using FacilityService.Infrastructure;
using FacilityService.Infrastructure.DBContext;
using FacilityService.Infrastructure.Seed;
using FacilityService.Infrastructure.Services.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FacilityService.IoC;

/// <summary>
/// ثبت همهٔ وابستگی‌های پنل Facility (مثل IdentityService.IoC.DependencyContainer).
/// Program.cs:  builder.Services.AddFacilityPanelServices(builder.Configuration);
/// احراز مجوز از Identity (AddFacilityPanelAuthorization) و Swagger در پروژهٔ Api می‌مانند.
/// </summary>
public static class DependencyContainer
{
    public static IServiceCollection AddFacilityPanelServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFacilityPanelApplication();            // MediatR + FacilityExceptionBehavior + TimeProvider
        services.AddFacilityPanelData(configuration);      // DbContext، ذخیره‌ساز فایل (Minio/محلی)
        return services;
    }
}
