using IdentityService.Contracts.Identities.CommandResponses;
using IdentityService.Contracts.Identities.CommandResponses.Common;
using IdentityService.Contracts.Identities.Repositories.Common;
using IdentityService.Domain.Models;
using IdentityService.Services.IServices;
using IdentityService.Services.ViewModels;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Handler.Identities.Services.OtpService;

public class OtpService<TSendOtpResponse, TVerifyOtpResponse>(
    IActiveTokenBaseCommandRepository activeTokenCommandRepository,
    IActiveTokenBaseQueryRepository activeTokenQueryRepository,
    ISmsService smsService,
    IConfiguration configuration) 
    where TSendOtpResponse : EmptyResponse
    where TVerifyOtpResponse : VerifyOtpBase, new()
{
    private readonly string _apiKeyInternal = configuration["ApiKeyInternal"]!;
    private readonly int _otpValidTimeSeconds = int.Parse(configuration["OtpValidTimeSeconds"]!);
    public async Task<BaseMessageResponse<TSendOtpResponse>> IdentifyWithPhoneNumber(string mobileNumber, CancellationToken cancellationToken)
    {
        var activationCode = await GenerateAndStoreActivationCodeAsync(mobileNumber, cancellationToken);
        return await SendOtpAsync(activationCode, cancellationToken);
    }
    
    private async Task<ActivationCodeModel> GenerateAndStoreActivationCodeAsync(string mobileNumber, CancellationToken cancellationToken)
    {
        var activationCode = new ActivationCodeModel(mobileNumber);
        activationCode.GenerateCode(false);
        activationCode.SetTime(DateTime.Now);
        
        await activeTokenCommandRepository.DeactivateActivationCodeAsync(activationCode, cancellationToken);
        await activeTokenCommandRepository.InsertActivationCodeAsync(activationCode, cancellationToken);
        return activationCode;
    }
    
    private async Task<BaseMessageResponse<TSendOtpResponse>> SendOtpAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken)
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
        return MessageResponse.GenerateMessageResponse<TSendOtpResponse>(responseStatus)!;
    }
    
    public async Task<BaseMessageResponse<TVerifyOtpResponse>> CheckOtpCodeVerificationAsync(string mobileNumber, string otpCode, CancellationToken cancellationToken)
    {
        var activationCode = new ActivationCodeModel(mobileNumber, otpCode);
        activationCode.SetTime(DateTime.Now);

        var status = await activeTokenQueryRepository.VerifyActivationCodeAsync(activationCode, _otpValidTimeSeconds, cancellationToken);
        var deactivateToken = new ActivationCodeModel(mobileNumber);
        if (status)
            await activeTokenCommandRepository.DeactivateActivationCodeAsync(deactivateToken, cancellationToken);

        return (!status ? MessageResponse.GenerateMessageResponse<TVerifyOtpResponse>(EResponseStatusCode.WrongParameters) : MessageResponse.GenerateMessageResponse<TVerifyOtpResponse>(EResponseStatusCode.DoneSuccessfully))!;
    }
}