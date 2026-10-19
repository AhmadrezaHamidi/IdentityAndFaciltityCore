using Amazon.Runtime.Internal.Util;
using AutoMapper;
using IdentityService.Application.Handler.Admin.Commands.AdminVerifyLoginOtp;
using IdentityService.Application.Handler.Admin.Services.RegisterLogin;
using IdentityService.Contracts.Admin.CommandResponses;
using IdentityService.Contracts.Admin.Repositories.ActiveTokens;
using IdentityService.Domain.IRepositories;
using IdentityService.Domain.Models;
using IdentityService.Domain.Models.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmidBank.Common;
using OmidBank.Common.Extentions;
using OmidBank.Common.JwtHelper;

namespace IdentityService.Application.Handler.Admin.Services.VerifyOtp;

public class VerifyOtpService(
    IActiveTokenCommandRepository activeTokenCommandRepository,
    IActiveTokenQueryRepository activeTokenQueryRepository,
    IUserRepository userRepository,
    IPersonTempRepository personTempRepository,
    RegisterLoginService registerLoginService,
    IMapper mapper,
    IConfiguration configuration,
    IJwtService jwtService,
    ILogger<AdminVerifyLoginOtpDto> _logger)
{
    private readonly string _applicationVersionV2 = configuration["ApplicationVersionV2"]!;
    private readonly string? _avatarFileServerUrl = configuration["AvatarFileServerUrl"]!;
    private readonly string _fileServerUrl = configuration["FileServiceUrl"]!;
    private readonly int _expierMinutesRegisterToken = int.Parse(configuration["ExpierMinutesRegisterToken"]);

    public async Task<BaseMessageResponse<AdminVerifyLoginOtpDto>> VerifyLoginOtp(AdminVerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetPanelUserByMobileNumberAsync(request.RequestInfo.MobileNumber, cancellationToken);
        
        var verificationFailure = await CheckOtpCodeVerificationFailureAsync(request, cancellationToken);
        if (verificationFailure is not null)
            return verificationFailure;

        if (user is null)
        {
            var registerToken = jwtService.GenerateAsync(_expierMinutesRegisterToken, request.RequestInfo.MobileNumber);
            return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, MakeNotRegisteredOtpDto(registerToken))!;
        }

        return MessageResponse.GenerateMessageResponse<AdminVerifyLoginOtpDto>(EResponseStatusCode.DoneSuccessfully, null);
    }

    private static AdminVerifyLoginOtpDto MakeNotRegisteredOtpDto(string token)
    {
        return new AdminVerifyLoginOtpDto(false, null, null, null,
            null, null, null, null, null,
            null, null, null, null, null,
            null, null, null,
            token, null);
    }
    
    private async Task<BaseMessageResponse<AdminVerifyLoginOtpDto>?> CheckOtpCodeVerificationFailureAsync(AdminVerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        var activationCode = new ActivationCodeModel(request.RequestInfo.MobileNumber, request.OtpCode);
        activationCode.SetTime(DateTime.Now);

        var status = await activeTokenQueryRepository.VerifyActivationCodeAsync(activationCode, cancellationToken);
        await activeTokenCommandRepository.DeactivateActivationCodeAsync(activationCode, cancellationToken);

        return (!status ? MessageResponse.GenerateMessageResponse<AdminVerifyLoginOtpDto>(EResponseStatusCode.WrongParameters)! : null)!;
    }

    private async Task GenerateTokenAsync(AdminVerifyLoginOtpCommand request, UserModel user, CancellationToken cancellationToken)
    {
        var activeToken = new ActiveTokenModel(request.RequestInfo.MobileNumber,
            request.RequestInfo.DeviceInfo,
            request.RequestInfo.Version, request.RequestInfo.IP, DateTime.Now.ToTimeStampSeconds(), true);

        activeToken.GenerateToken();

        await activeTokenCommandRepository.DeactivateActiveTokenAsync(activeToken, cancellationToken);
        await activeTokenCommandRepository.InsertActiveTokenAsync(activeToken, cancellationToken);
        user.SetToken(activeToken.Token);
    }

    private async Task UpdateTempUserApplicationVersionAsync(AdminVerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        var applicationVersion = !string.IsNullOrEmpty(request.RequestInfo.Version) ? request.RequestInfo.Version : _applicationVersionV2;
        var personTempInfo = await personTempRepository.GetPersonTempInfo(request.RequestInfo.MobileNumber, cancellationToken);
        if (personTempInfo is not null)
            await personTempRepository.UpdateApplicationVersion(personTempInfo.Id, applicationVersion, cancellationToken);
    }

    private string MakeAvatarUrl(AdminVerifyLoginOtpCommand request, UserModel user)
    {
        string avatarUrl;
        if (request.RequestInfo.AppId == Enums.AppEnum.MobileApp)
            avatarUrl = user.AvatarName != null ? $"{_fileServerUrl}/{user.AvatarName}" : null;
        else
            avatarUrl = (user.AvatarName != null ? (request.RequestInfo.AppId != Enums.AppEnum.MobileApp) ?
                    _avatarFileServerUrl
                    : $"{_fileServerUrl}/{user.AvatarName}"
                : "")!;
        return avatarUrl;
    }


    private async Task GenerateUserInvitedCode(UserModel user, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(user.MyInviteCode))
            return;

        var myInviteCode = InviteCode.Generate(user.NationalCode, Enums.InviteType.User);
        user.MyInviteCode = myInviteCode;
        await userRepository.UpdateMyInviteCodeAsync(user.PersonId, myInviteCode, cancellationToken);
        await userRepository.SaveChangeAsync(cancellationToken);
    }
}