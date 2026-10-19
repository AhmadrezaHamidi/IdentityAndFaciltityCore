using IdentityService.Application.Handler.Admin.Commands.AdminLoginWithPhoneNumber;
using IdentityService.Contracts.Admin.CommandResponses;
using IdentityService.Contracts.Admin.Repositories.ActiveTokens;
using IdentityService.Domain.Models;
using IdentityService.Services.IServices;
using IdentityService.Services.ViewModels;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Handler.Admin.Services.Login;

public class LoginService(
    IActiveTokenCommandRepository activeTokenCommandRepository,
    ISmsService smsService,
    IConfiguration configuration
    )
{
    private readonly string _apiKeyInternal = configuration["ApiKeyInternal"]!;
    public async Task<BaseMessageResponse<AdminLoginWithPhoneNumberDto>> LoginWithPhoneNumber(AdminLoginWithPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var response = CheckClientVersion(request.RequestInfo.Version, request.RequestInfo.AppId);
        if (response is not null)
            return response;
        var activationCode = await GenerateAndStoreActivationCodeAsync(request, cancellationToken);
        return await SendOtpAsync(activationCode, cancellationToken);
    }

    private static BaseMessageResponse<AdminLoginWithPhoneNumberDto>? CheckClientVersion(string? version, Enums.AppEnum appId)
    {
        //double requestVersion = 0;
        //if (!string.IsNullOrEmpty(version))
        //    requestVersion = double.Parse(version);

        //if (requestVersion <= 2.4 && appId == Enums.AppEnum.MobileApp)
        //    return MessageResponse.GenerateMessageResponse<LoginWithPhoneNumberDto>(EResponseStatusCode.LoginUpdateMessage)!;
        return null;
    }

  
    private async Task<ActivationCodeModel> GenerateAndStoreActivationCodeAsync(AdminLoginWithPhoneNumberCommand request, CancellationToken cancellationToken)
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

    private async Task<BaseMessageResponse<AdminLoginWithPhoneNumberDto>> SendOtpAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken)
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
        return MessageResponse.GenerateMessageResponse<AdminLoginWithPhoneNumberDto>(responseStatus)!;
    }
}