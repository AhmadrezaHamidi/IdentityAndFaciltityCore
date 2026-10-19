using IdentityService.Contracts.Identities.Repositories.ActiveTokens;
using IdentityService.Contracts.Identities.Repositories.PasswordChangeActiveToken;
using IdentityService.Contracts.Identities.Repositories.UsernameChangeActiveToken;
using IdentityService.Contracts.RegexValidation.Repositories;
using IdentityService.Domain.Services.ContentManagement;
using IdentityService.Domain.Services.UserManagementPanel;
using IdentityService.Domain.Authorization;
using IdentityService.Infrastructure.Contexts;
using IdentityService.Infrastructure.Repositories.ContentManagement;
using IdentityService.Infrastructure.Repositories.Identities.ActiveTokens;
using IdentityService.Infrastructure.Repositories.Identities.PasswordChangeActiveTokens;
using IdentityService.Infrastructure.Repositories.Identities.UsernameChangeActiveTokens;
using IdentityService.Infrastructure.Repositories.RegexValidation;
using IdentityService.Infrastructure.Repositories.UserManagement;
using IdentityService.Infrastructure.Repositories.UserManagementPanel;
using Microsoft.Extensions.DependencyInjection;
using OmidBank.Common.Cache.MemoryCache;
using OmidBank.Common.Cookie;
using OmidBank.Common.Helper.Minio;

namespace IdentityService.Infrastructure;

public static class DependencyInjectionExtension
{
    public static IServiceCollection InfrastructureRegisterServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContextPool<ApplicationDBContext>((options) =>
        {
            options.UseSqlServer( configuration.GetConnectionString("IdentityConnection"));
        });

        services.AddScoped<ApplicationDBContextInitialiser>();
        services.AddScoped<IActiveTokenCommandRepository, ActiveTokenCommandRepository>();
        services.AddScoped<IActiveTokenQueryRepository, ActiveTokenQueryRepository>();

        services.AddScoped<IRegexValidationRepository, RegexValidationRepository>();
        services.AddScoped<IUsernameChangeActiveTokenCommandRepository, UsernameChangeActiveTokenCommandRepository>();
        services.AddScoped<IUsernameChangeActiveTokenQueryRepository, UsernameChangeActiveTokenQueryRepository>();
        services.AddScoped<IPasswordChangeActiveTokenCommandRepository, PasswordChangeActiveTokenCommandRepository>();
        services.AddScoped<IPasswordChangeActiveTokenQueryRepository, PasswordChangeActiveTokenQueryRepository>();
        
        services.AddScoped<IUserManagementKycTransactionRepository, UserManagementKycTransactionRepository>();
        services.AddSingleton<ICacheManager, MemoryCacheManager>();
        services.AddSingleton<ICookieManager, CookieManager>();


        services.AddScoped<IPanelAccessControlRepository, PanelAccessControlRepository>();

        // توکن ← کاربر پنل با مجوزهای لحظه‌ای. PanelPermissionFilter بدون این، هر درخواست را
        // رد می‌کند (جهت امن، ولی پنل کار نمی‌کند).
        services.AddScoped<IPanelSessionResolver, PanelSessionResolver>();



        services.AddScoped<IContentManagementRepository, ContentManagementRepository>();

        services.AddHttpClient("ShahkarClient", client =>
        {
            client.BaseAddress = new Uri(configuration["ShahkarProviders:BaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        //services.AddHttpClient<IBulkShahkarVerifier, BulkShahkarVerifier>();

        services.Configure<MinioOptions>(configuration.GetSection("Minio"));
        return services;
    }
}