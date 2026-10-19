using IdentityService.Application.Handler.Identities.Commands.LoginWithPhoneNumber;
using IdentityService.Contracts.Identities.CommandResponses;
using IdentityService.Contracts.Identities.Repositories.ActiveTokens;
using IdentityService.Domain.Models;
using IdentityService.Services.IServices;
using IdentityService.Services.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmidBank.Common.Cache;
using OmidBank.Common.JwtHelper;

namespace IdentityService.Application.Handler.Identities.Services.Login;

public class LoginService(
    IActiveTokenCommandRepository activeTokenCommandRepository,
    ISmsService smsService,
    IConfiguration configuration,
    IRedisCacheManager redisCacheManager,
    IJwtService jwtService,
    ILogger<LoginService> logger
    )
{
    private readonly string _apiKeyInternal = configuration["ApiKeyInternal"]!;
    public async Task<BaseMessageResponse<LoginWithPhoneNumberDto>> LoginWithPhoneNumber(LoginWithPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        //var versionName = request.RequestInfo.Version;
        //var version = int.Parse(versionName.Replace(".", ""));

        if (request.RequestInfo.AppId == Enums.AppEnum.MobileApp /*&& version >= 272*/)
        {
            var vaildateRegisterTokenStatus = jwtService.ValidateRegisterToken(request.InitToken);

            if (!vaildateRegisterTokenStatus.Item2 || vaildateRegisterTokenStatus.Item1.MobileNumber != request.MobileNumber)
            {
                var responseUnAuthorize = MessageResponse.GenerateMessageResponse<LoginWithPhoneNumberDto>(EResponseStatusCode.UnAuthorized);
                responseUnAuthorize.Message = "عدم دسترسی";
                responseUnAuthorize.Data = null;
                return responseUnAuthorize;
            }

            var randomStringInfo = redisCacheManager.Read(string.Format(CacheKey.InitToken, request.InitToken));

            if (randomStringInfo.ToString() is null)
            {
                var responseUnAuthorize = MessageResponse.GenerateMessageResponse<LoginWithPhoneNumberDto>(EResponseStatusCode.UnAuthorized);
                responseUnAuthorize.Message = "عدم دسترسی";
                responseUnAuthorize.Data = null;
                return responseUnAuthorize;
            }

            var randomStringValue = redisCacheManager.Read(string.Format(CacheKey.InitRandomString, randomStringInfo));

            if (!string.IsNullOrEmpty(randomStringValue))
            {
                var responseUnAuthorize = MessageResponse.GenerateMessageResponse<LoginWithPhoneNumberDto>(EResponseStatusCode.UnAuthorized);
                responseUnAuthorize.Message = "عدم دسترسی";
                responseUnAuthorize.Data = null;
                return responseUnAuthorize;
            }


            redisCacheManager.Write(string.Format(CacheKey.InitRandomString, randomStringInfo), request.RequestInfo.MobileNumber, Enums.ECacheTime.OneWeek);
        }

        var activationCode = await GenerateAndStoreActivationCodeAsync(request, cancellationToken);
        return await SendOtpAsync(activationCode, cancellationToken);
    }

    private static BaseMessageResponse<LoginWithPhoneNumberDto>? CheckClientVersion(string? version, Enums.AppEnum appId)
    {
        //double requestVersion = 0;
        //if (!string.IsNullOrEmpty(version))
        //    requestVersion = double.Parse(version);

        //if (requestVersion <= 2.4 && appId == Enums.AppEnum.MobileApp)
        //    return MessageResponse.GenerateMessageResponse<LoginWithPhoneNumberDto>(EResponseStatusCode.LoginUpdateMessage)!;
        return null;
    }


    private async Task<ActivationCodeModel> GenerateAndStoreActivationCodeAsync(LoginWithPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var activationCode = new ActivationCodeModel(request.MobileNumber)
        {
            Type = Enums.ActivationCode.Login
        };

        activationCode.GenerateCode(false);
        activationCode.SetTime(DateTime.Now);

        await activeTokenCommandRepository.DeactivateActivationCodeByMobileNumber(request.MobileNumber, cancellationToken);
        await activeTokenCommandRepository.InsertActivationCodeAsync(activationCode, cancellationToken);
        return activationCode;
    }

    private async Task<BaseMessageResponse<LoginWithPhoneNumberDto>> SendOtpAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken)
    {
        var messageServiceUrl = configuration["MessageServiceUrl"];
        var sendSmsRequest = new SendActivitionCodeOmidTechViewModel
        {
            MobileNumber = activationCode.MobileNumber,
            Scheduler = null,
            Code = activationCode.Code,
            Url = messageServiceUrl,
            ApiKeyInternal = _apiKeyInternal
        };
        var activationSmsStatus = await smsService.SendActivationCodeFromThirdPartyOmidTech(sendSmsRequest, cancellationToken);

        var responseStatus = activationSmsStatus
            ? EResponseStatusCode.DoneSuccessfully
            : EResponseStatusCode.InternalServerError;
        return MessageResponse.GenerateMessageResponse<LoginWithPhoneNumberDto>(responseStatus)!;
    }
}