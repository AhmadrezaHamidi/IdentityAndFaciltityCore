using IdentityService.Application.Handler.Admin.Services;
using IdentityService.Application.Handler.Identities.Services;
using IdentityService.Application.Handler.Identities.Services.ActivationCode;
using IdentityService.Application.Handler.Identities.Services.ChangeUserActiveState;
using IdentityService.Application.Handler.Identities.Services.Login;
using IdentityService.Application.Handler.Identities.Services.OtpService;
using IdentityService.Application.Handler.Identities.Services.Register;
using IdentityService.Application.Handler.Identities.Services.RegisterLogin;
using IdentityService.Application.Handler.Identities.Services.VerifyOtp;
using IdentityService.Application.Handler.RegexValidations.Service;
using IdentityService.Application.Services.ChangeMobileNumber;
using IdentityService.Application.Services.Kyc.Common;
using IdentityService.Application.Services.Kyc.Common.Interfaces;
using IdentityService.Application.Services.Kyc.Factories;
using IdentityService.Application.Services.Kyc.Farashenasa;
using IdentityService.Application.Services.Kyc.Vinara;
using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement;
using IdentityService.Application.Services.LocalJwt;
using IdentityService.Application.Services.RedisBruteForceDetections;
using IdentityService.Contracts.Services.BruteForceDetection;
using IdentityService.Contracts.Services.BruteForceDetection.Options;
using IdentityService.Contracts.Services.JwtService;
using IdentityService.Services.IServices;
using IdentityService.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OmidBank.Common.Helper.Minio;
using OmidBank.Common.InternalThreadParty.BankService;
using OmidBank.Common.InternalThreadParty.OmidTech;
using System.Reflection;

namespace IdentityService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<LoginService>();
        services.AddScoped<VerifyOtpService>();
        services.AddScoped<RegisterService>();
        services.AddScoped<RegisterLoginService>();
        services.AddScoped<IIdentityService, IdentityClassService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<RequestHelperV2>();

        services.AddScoped<IActivationCodeService, ActivationCodeService>();

        services.AddScoped<IChangeUserActiveStateService, ChangeUserActiveStateService>();

        services.AddScoped<ChangeUsernameOtpService>();
        services.AddScoped<ChangePasswordOtpService>();

        services.AddScoped<FarashenasaKycRegisterUmServiceOld>();

        services.AddScoped<IRegexValidationService, RegexValidationService>();
        services.AddScoped<IJwtService>(_ =>
        {
            var issuer = configuration["JwtSettings:Issuer"]!;
            var audience = configuration["JwtSettings:Audience"]!;
            var secretKey = configuration["JwtSettings:JwtSecretKey"]!;
            return new LocalJwtService(secretKey, issuer, audience);
        });
        services.Configure<BruteForceOptions>(configuration.GetSection("BruteForceOptions:LoginVerify"));
        services.AddSingleton<IBruteForceDetection, RedisBruteForceDetection>();
        services.AddScoped<VinaraWageService>();
        services.AddScoped<KycResultManagement>();
        services.AddScoped<KycSmsHelper>();
        services.AddScoped<VinaraPushVerificationService>();
        services.AddScoped<RetrieveUserInformationService>();
        services.AddScoped<IKycServiceProvider, FarashenasaKycService>();
        services.AddScoped<IKycServiceProvider, VinaraKycService>();
        services.AddScoped<IKycServiceFactory, KycServiceFactory>();
        services.AddScoped<ImageStorageService>();
        services.AddScoped<FileBasedStorage>();
        services.AddScoped<VinaraKycTransactionService>();

        services.AddScoped<IKycRegisterUmService, FarashenasaKycRegisterUmService>();
        services.AddScoped<IKycRegisterUmService, VinaraKycRegisterUmService>();
        services.AddScoped<IKycRegisterUmServiceFactory, KycRegisterUmServiceFactory>();

        services.AddScoped<OmidTechProvider>(provider =>
        {
            var httpClient = provider.GetRequiredService<HttpClient>();
            return new OmidTechProvider(configuration["BaseOmidTechUrl"], configuration["ApiKeyOmidTech"], httpClient);
        });

        services.AddScoped<IAdminIdentityService, AdminIdentityClassService>();
        services.AddScoped<Handler.Admin.Services.Login.LoginService>();
        services.AddScoped<Handler.Admin.Services.VerifyOtp.VerifyOtpService>();
        services.AddScoped<Handler.Admin.Services.RegisterLogin.RegisterLoginService>();

        services.AddScoped<IChangeMobileNumberService, ChangeMobileNumberService>();
        services.AddScoped<BankServiceProvider>(_ => new BankServiceProvider(configuration["BankServiceUrl"], configuration["ApiKeyInternal"]));

        return services;
    }
}