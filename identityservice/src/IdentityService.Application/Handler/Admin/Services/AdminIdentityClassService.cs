using IdentityService.Application.Handler.Admin.Commands.AdminLoginWithPhoneNumber;
using IdentityService.Application.Handler.Admin.Commands.AdminVerifyLoginOtp;
using IdentityService.Application.Handler.Admin.Services.Login;
using IdentityService.Application.Handler.Admin.Services.OtpService;
using IdentityService.Application.Handler.Admin.Services.VerifyOtp;
using IdentityService.Application.Handler.RegexValidations.Service;
using IdentityService.Contracts.Admin.CommandResponses;
using IdentityService.Contracts.Admin.CommandResponses.Common;
using IdentityService.Contracts.Services.JwtService;
using IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Entities;
using IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Enums;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.ExternalThirdParty.Tosan;
using OmidBank.Common.ExternalThirdParty.Tosan.Setting;
using OmidBank.Common.ExternalThirdParty.Tosan.Setting.Request;

namespace IdentityService.Application.Handler.Admin.Services;

public class AdminIdentityClassService(
    IRegexValidationService regexValidationService,
    LoginService loginService,
    VerifyOtpService verifyOtpService,
    IJwtService jwtService,
    TosanSettingProvider tosanSettingProvider,
    IConfiguration configuration) : IAdminIdentityService
{
    private const string PasswordPropertyName = "Password";
    private const string UsernamePropertyName = "Username";
    
    private readonly int _tokenValidTimeSeconds = int.Parse(configuration["TokenValidTimeSeconds"]!);
    
    public async Task<BaseMessageResponse<AdminLoginWithPhoneNumberDto>> AdminLoginWithPhoneNumber(AdminLoginWithPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        return await loginService.LoginWithPhoneNumber(request, cancellationToken);
    }
    
    public async Task<BaseMessageResponse<AdminVerifyLoginOtpDto>> AdminVerifyLoginOtp(AdminVerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        return await verifyOtpService.VerifyLoginOtp(request, cancellationToken);
    }
}