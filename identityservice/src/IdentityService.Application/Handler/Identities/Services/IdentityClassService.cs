using IdentityService.Application.Handler.Identities.Commands.ChangePassword;
using IdentityService.Application.Handler.Identities.Commands.ChangeUsername;
using IdentityService.Application.Handler.Identities.Commands.LoginWithPhoneNumber;
using IdentityService.Application.Handler.Identities.Commands.RegisterUser;
using IdentityService.Application.Handler.Identities.Commands.SendPasswordChangeOtp;
using IdentityService.Application.Handler.Identities.Commands.SendUsernameChangeOtp;
using IdentityService.Application.Handler.Identities.Commands.VerifyLoginOtp;
using IdentityService.Application.Handler.Identities.Commands.VerifyPasswordChangeOtp;
using IdentityService.Application.Handler.Identities.Commands.VerifyUsernameChangeOtp;
using IdentityService.Application.Handler.Identities.Queries.RetrieveSettingConfigurations;
using IdentityService.Application.Handler.Identities.Services.Login;
using IdentityService.Application.Handler.Identities.Services.OtpService;
using IdentityService.Application.Handler.Identities.Services.Register;
using IdentityService.Application.Handler.Identities.Services.VerifyOtp;
using IdentityService.Application.Handler.RegexValidations.Service;
using IdentityService.Contracts.Identities.CommandResponses;
using IdentityService.Contracts.Identities.CommandResponses.Common;
using IdentityService.Contracts.Identities.ReadModels;
using IdentityService.Contracts.Identities.Repositories.UserManagementSettingChangeHistories;
using IdentityService.Contracts.Services.JwtService;
using IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Entities;
using IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Enums;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.ExternalThirdParty.Tosan;
using OmidBank.Common.ExternalThirdParty.Tosan.Setting;
using OmidBank.Common.ExternalThirdParty.Tosan.Setting.Request;

namespace IdentityService.Application.Handler.Identities.Services;

public class IdentityClassService(
    IUserManagementSettingChangeHistoryCommandRepository userManagementSettingChangeHistoryCommandRepository,
    IRegexValidationService regexValidationService,
    LoginService loginService,
    VerifyOtpService verifyOtpService,
    RegisterService registerService,
    ChangeUsernameOtpService changeUsernameOtpService,
    ChangePasswordOtpService changePasswordOtpService,
    IJwtService jwtService,
    TosanSettingProvider tosanSettingProvider,
    IConfiguration configuration) : IIdentityService
{
    private const string PasswordPropertyName = "Password";
    private const string UsernamePropertyName = "Username";
    
    private readonly int _tokenValidTimeSeconds = int.Parse(configuration["TokenValidTimeSeconds"]!);
    
    public async Task<BaseMessageResponse<LoginWithPhoneNumberDto>> LoginWithPhoneNumber(LoginWithPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        return await loginService.LoginWithPhoneNumber(request, cancellationToken);
    }
    
    public async Task<BaseMessageResponse<RegisterUserDto>> RegisterUser(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await registerService.RegisterUser(request, cancellationToken);
    }
    
    public async Task<BaseMessageResponse<VerifyLoginOtpDto>> VerifyLoginOtp(VerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        return await verifyOtpService.VerifyLoginOtp(request, cancellationToken);
    }

    public async Task<BaseMessageResponse<SendUsernameChangeOtpDto>> SendUsernameChangeOtp(SendUsernameChangeOtpCommand request, CancellationToken cancellationToken)
    { 
        var result = await changeUsernameOtpService.IdentifyWithPhoneNumber(request.RequestInfo.MobileNumber, cancellationToken);
        return result;
    }

    public async Task<BaseMessageResponse<VerifyUsernameChangeOtpDto>> VerifyUsernameChangeOtp(VerifyUsernameChangeOtpCommand request, CancellationToken cancellationToken)
    {
        var result = await changeUsernameOtpService.CheckOtpCodeVerificationAsync(request.RequestInfo.MobileNumber, request.OtpCode, cancellationToken);
        result.Data = await GenerateResourceBasedToken<VerifyUsernameChangeOtpDto>(request.CurrentUser!.Id, request.RequestInfo.MobileNumber, EUserManagementSettingOperation.ChangeUsername, cancellationToken);
        return result;
    }

    public async Task<BaseMessageResponse<SendPasswordChangeOtpDto>> SendPasswordChangeOtp(SendPasswordChangeOtpCommand request, CancellationToken cancellationToken)
    {
        var result = await changePasswordOtpService.IdentifyWithPhoneNumber(request.RequestInfo.MobileNumber, cancellationToken);
        return result;
    }

    public async Task<BaseMessageResponse<VerifyPasswordChangeOtpDto>> VerifyPasswordChangeOtp(VerifyPasswordChangeOtpCommand request, CancellationToken cancellationToken)
    {
        var result = await changePasswordOtpService.CheckOtpCodeVerificationAsync(request.RequestInfo.MobileNumber, request.OtpCode, cancellationToken);
        result.Data = await GenerateResourceBasedToken<VerifyPasswordChangeOtpDto>(request.CurrentUser!.Id, request.RequestInfo.MobileNumber, EUserManagementSettingOperation.ChangePassword, cancellationToken);
        return result;
    }

    private async Task<TData> GenerateResourceBasedToken<TData>(long userId, string mobileNumber, EUserManagementSettingOperation operation, CancellationToken cancellationToken) where TData : VerifyOtpBase, new()
    {
        var history = UserManagementSettingChangeHistory.Create(userId, operation);
        history = await userManagementSettingChangeHistoryCommandRepository.InsertAsync(history, cancellationToken);
        var claims = new Dictionary<string, object>
        {
            ["MobileNumber"] = mobileNumber,
            ["ResourceId"] = history.Id,
        };
        var data = new TData
        {
            Token = jwtService.GenerateToken(claims, TimeSpan.FromSeconds(_tokenValidTimeSeconds))
        };
        return data;
    }

    private async Task<BaseMessageResponse<TResponse>> ChangeSetting<TResponse>(string token, string mobileNumber, string property, string newSetting, Func<Task<TosanExceptionDto?>> doChangeSetting, CancellationToken cancellationToken) where TResponse : ChangeSettingDto, new()
    {
        var tokenResult = ValidateChangeSettingToken<TResponse>(token, mobileNumber);
        if (!tokenResult.Item1.IsSuccess)
            return tokenResult.Item1;

        var regexValidation = await regexValidationService.ValidatePropertyAsync(property, newSetting, cancellationToken);
        if (!regexValidation.IsValid)
            return MessageResponse.GenerateMessageResponse(EResponseStatusCode.WrongParameters, new TResponse { Message = regexValidation.Message, Success = false })!;
        
        var history = await userManagementSettingChangeHistoryCommandRepository.RetrieveAsync(tokenResult.Item2, EUserManagementSettingChangeStatus.Unknown, cancellationToken);
        if (history is null)
            return MessageResponse.GenerateMessageResponse<TResponse>(EResponseStatusCode.WrongParameters)!;
        
        var exception = await doChangeSetting();
        if (exception is null)
        {
            history.SetAsSuccessful();
            await userManagementSettingChangeHistoryCommandRepository.UpdateStatusAsync(history, cancellationToken);
            return MessageResponse.GenerateMessageResponse<TResponse>(EResponseStatusCode.DoneSuccessfully)!;
        }
        var result = MessageResponse.GenerateMessageResponse<TResponse>(EResponseStatusCode.CallErrorTosan)!;
        result.Message = exception.TranslateMessage ?? exception.TranslateExceptionMessage;
        history.SetAsFailed(result.Message);
        await userManagementSettingChangeHistoryCommandRepository.UpdateStatusAsync(history, cancellationToken);
        return result!;
    }

    public async Task<BaseMessageResponse<ChangePasswordDto>> ChangePassword(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await ChangeSetting<ChangePasswordDto>(request.Token, request.RequestInfo.MobileNumber, 
            PasswordPropertyName, request.NewPassword, async () =>
            {
                var changePasswordRequest = MakeChangePasswordRequest(request);
                var response = await tosanSettingProvider.ChangePasswordAsync(changePasswordRequest, cancellationToken);
                return response?.Exception;
            }, cancellationToken);
        return result;
    }

    private static ChangePasswordRequest MakeChangePasswordRequest(ChangePasswordCommand request)
    {
        return new ChangePasswordRequest(request.CurrentUser!.TosanToken)
        {
            CurrentPassword = ***;
    }

    public async Task<BaseMessageResponse<ChangeUsernameDto>> ChangeUsername(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        var result = await ChangeSetting<ChangeUsernameDto>(request.Token, request.RequestInfo.MobileNumber, 
            UsernamePropertyName, request.NewUsername, async () =>
            {
                var changeUsernameRequest = MakeChangeUsernameRequest(request);
                var response = await tosanSettingProvider.ChangeUsernameAsync(changeUsernameRequest, cancellationToken);
                return response?.Exception;
            }, cancellationToken);
        return result;
    }

    private static ChangeUsernameRequest MakeChangeUsernameRequest(ChangeUsernameCommand request)
    {
        return new ChangeUsernameRequest(request.CurrentUser!.TosanToken)
        {
            CurrentUsername = request.CurrentUsername,
            NewUsername = request.NewUsername,
        };
    }
    
    public Task<BaseMessageResponse<RetrieveSettingConfigurationsDto>> RetrieveSettingConfigurations(RetrieveSettingConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var warning = configuration["UserSettings:WarningMessage"];
        var result = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, new RetrieveSettingConfigurationsDto(warning!));
        return Task.FromResult(result)!;
    }

    private (BaseMessageResponse<TResponse>, long) ValidateChangeSettingToken<TResponse>(string token, string mobileNumber)
    {
        var claimsPrincipal = jwtService.ValidateToken(token);
        if (claimsPrincipal!.Claims.FirstOrDefault(x => x.Type == "MobileNumber")?.Value != mobileNumber)
            return (MessageResponse.GenerateMessageResponse<TResponse>(EResponseStatusCode.WrongParameters)!, -1)!;

        var idStr = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "ResourceId")?.Value;
        return (!long.TryParse(idStr, out var id) ? 
            (MessageResponse.GenerateMessageResponse<TResponse>(EResponseStatusCode.WrongParameters), -1) : 
            (MessageResponse.GenerateMessageResponse<TResponse>(EResponseStatusCode.DoneSuccessfully), id))!;
    }
}