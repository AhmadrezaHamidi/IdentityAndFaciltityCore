using Microsoft.Extensions.DependencyInjection;

namespace FacilityService.Application.Handler.Shared;

/// <summary>
/// ثبت لایهٔ Application پنل. Program.cs:  builder.Services.AddFacilityPanelApplication();
/// اگر پروژه MediatR را از همین اسمبلی ثبت کرده، فقط Behavior و TimeProvider را اضافه کن.
/// </summary>
public static class FacilityPanelApplicationRegistration
{
    public static IServiceCollection AddFacilityPanelApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<FacilityPanelHandlerBase>();
            cfg.AddOpenBehavior(typeof(FacilityExceptionBehavior<,>));
        });
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}
