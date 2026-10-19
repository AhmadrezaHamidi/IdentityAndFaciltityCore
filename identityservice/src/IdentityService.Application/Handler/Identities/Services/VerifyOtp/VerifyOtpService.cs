using AutoMapper;
using IdentityService.Application.Handler.Identities.Commands.VerifyLoginOtp;
using IdentityService.Application.Handler.Identities.Services.RegisterLogin;
using IdentityService.Contracts.Identities.CommandResponses;
using IdentityService.Contracts.Identities.Repositories.ActiveTokens;
using IdentityService.Domain.IRepositories;
using IdentityService.Domain.Models;
using IdentityService.Domain.Models.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmidBank.Common.Cache;
using OmidBank.Common.Extentions;
using OmidBank.Common.JwtHelper;

namespace IdentityService.Application.Handler.Identities.Services.VerifyOtp;

public class VerifyOtpService(
    IActiveTokenCommandRepository activeTokenCommandRepository,
    IActiveTokenQueryRepository activeTokenQueryRepository,
    IUserRepository userRepository,
    IPersonTempRepository personTempRepository,
    RegisterLoginService registerLoginService,
    IMapper mapper,
    IConfiguration configuration,
    IJwtService jwtService,
    ILogger<VerifyLoginOtpDto> _logger,
        IRedisCacheManager redisCacheManager
)
{
    private readonly string _applicationVersionV2 = configuration["ApplicationVersionV2"]!;
    private readonly string? _avatarFileServerUrl = configuration["AvatarFileServerUrl"]!;
    private readonly string _fileServerUrl = configuration["FileServiceUrl"]!;
    private readonly int _expierMinutesRegisterToken = int.Parse(configuration["ExpierMinutesRegisterToken"]);

    public async Task<BaseMessageResponse<VerifyLoginOtpDto>> VerifyLoginOtp(VerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {

        if (request.RequestInfo.AppId == Enums.AppEnum.MobileApp)
        {
            var vaildateRegisterTokenStatus = jwtService.ValidateRegisterToken(request.InitToken);
            if (!vaildateRegisterTokenStatus.Item2 || vaildateRegisterTokenStatus.Item1.MobileNumber != request.RequestInfo.MobileNumber)
            {
                var responseUnAuthorize = MessageResponse.GenerateMessageResponse<VerifyLoginOtpDto>(EResponseStatusCode.UnAuthorized);
                responseUnAuthorize.Message = "عدم دسترسی";
                responseUnAuthorize.Data = null;
                return responseUnAuthorize;
            }
            var randomStringInfo = redisCacheManager.Read(string.Format(CacheKey.InitToken, request.InitToken));
            if (randomStringInfo.ToString() is null)
            {
                var responseUnAuthorize = MessageResponse.GenerateMessageResponse<VerifyLoginOtpDto>(EResponseStatusCode.UnAuthorized);
                responseUnAuthorize.Message = "عدم دسترسی";
                responseUnAuthorize.Data = null;
                return responseUnAuthorize;
            }
        }

        var userModel = await userRepository.GetByMobileNumberAsync(request.RequestInfo.MobileNumber, cancellationToken);
        var verificationFailure = await CheckOtpCodeVerificationFailureAsync(request, cancellationToken);
        if (verificationFailure is not null)
            return verificationFailure;

        if (userModel is null)
        {
            var registerToken = jwtService.GenerateAsync(_expierMinutesRegisterToken, request.RequestInfo.MobileNumber);
            return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, MakeNotRegisteredOtpDto(request.RequestInfo.MobileNumber, registerToken, false))!;
        }

        await GenerateTokenAsync(request, userModel, cancellationToken);

        var responseUpdateVersionPersonTemp = UpdateTempUserApplicationVersionAsync(request, cancellationToken);
        var avatarUrl = MakeAvatarUrl(request, userModel);
        var responseGenerateInviteCode = GenerateUserInvitedCode(userModel, cancellationToken);

        var city = await registerLoginService.GetCityInformation(userModel.CityId, cancellationToken);
        var resultTemp = registerLoginService.MakeResponse(userModel, city, avatarUrl, userModel.Token, true, cancellationToken);
        Task.WaitAll(responseGenerateInviteCode, responseUpdateVersionPersonTemp, resultTemp);

        var userIsActive = await userRepository.IsActive(userModel.Id, cancellationToken);

        var result = mapper.Map<VerifyLoginOtpDto>(resultTemp.Result);

        if (!userIsActive)
        {
            var registerToken = jwtService.GenerateAsync(_expierMinutesRegisterToken, request.RequestInfo.MobileNumber);
            return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, MakeNotRegisteredOtpDto(request.RequestInfo.MobileNumber, registerToken, userIsActive))!;
        }

        result.UserIsActive = userIsActive;
        return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, result)!;
    }

    private static VerifyLoginOtpDto MakeNotRegisteredOtpDto(string mobileNumber, string token, bool userIsActive)
    {
        var verifyLoginOtpDto = new VerifyLoginOtpDto(false, null, null, null,
            null, null, null, null, null,
            null, mobileNumber, null, null, null,
            null, null, null,
            token, null);
        verifyLoginOtpDto.UserIsActive = userIsActive;
        return verifyLoginOtpDto;
    }

    private async Task<BaseMessageResponse<VerifyLoginOtpDto>?> CheckOtpCodeVerificationFailureAsync(VerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        var activationCode = new ActivationCodeModel(request.RequestInfo.MobileNumber, request.OtpCode);
        activationCode.SetTime(DateTime.Now);

        var status = await activeTokenQueryRepository.VerifyActivationCodeAsync(activationCode, cancellationToken);
        await activeTokenCommandRepository.DeactivateActivationCodeAsync(activationCode, cancellationToken);

        return (!status ? MessageResponse.GenerateMessageResponse<VerifyLoginOtpDto>(EResponseStatusCode.WrongParameters)! : null)!;
    }

    private async Task GenerateTokenAsync(VerifyLoginOtpCommand request, UserModel user, CancellationToken cancellationToken)
    {
        var activeToken = new ActiveTokenModel(request.RequestInfo.MobileNumber,
            request.RequestInfo.DeviceInfo,
            request.RequestInfo.Version, request.RequestInfo.IP, DateTime.Now.ToTimeStampSeconds(), true);

        activeToken.GenerateToken();

        await activeTokenCommandRepository.DeactivateActiveTokenAsync(activeToken, cancellationToken);
        await activeTokenCommandRepository.InsertActiveTokenAsync(activeToken, cancellationToken);
        user.SetToken(activeToken.Token);
    }

    private async Task UpdateTempUserApplicationVersionAsync(VerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        var applicationVersion = !string.IsNullOrEmpty(request.RequestInfo.Version) ? request.RequestInfo.Version : _applicationVersionV2;
        var personTempInfo = await personTempRepository.GetPersonTempInfo(request.RequestInfo.MobileNumber, cancellationToken);
        if (personTempInfo is not null)
            await personTempRepository.UpdateApplicationVersion(personTempInfo.Id, applicationVersion, cancellationToken);
    }

    private string MakeAvatarUrl(VerifyLoginOtpCommand request, UserModel user)
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