using IdentityService.Application.Handler.Identities.Commands.ChangeUserActiveState.DeactiveUserState;
using IdentityService.Application.Handler.Identities.Commands.ChangeUserActiveState.VerifyChangeUserStateOtp;
using IdentityService.Application.Handler.Identities.Services.ChangeUserActiveState;
using IdentityService.Contracts.Identities.Repositories.ActiveTokens;
using IdentityService.Domain.Entity.UserManagement.ChangeUserActiveStateHistory;
using IdentityService.Domain.IRepositories;
using IdentityService.Domain.Models;
using IdentityService.Domain.Models.User;
using IdentityService.Services.IServices;
using IdentityService.Services.ViewModels;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.Cache;

namespace IdentityService.Application.Handler.Identities.Services.Register;

public class ChangeUserActiveStateService(IUserRepository userRepository, IConfiguration configuration, ISmsService smsService,
    IActiveTokenCommandRepository activeTokenCommandRepository, IActiveTokenQueryRepository activeTokenQueryRepository, IActiveTokenRepository activeTokenRepository,
    IChangeUserActiveStateRepository changeUserActiveStateRepository, IRedisCacheManager redisCacheManager) : IChangeUserActiveStateService
{
    private readonly string _apiKeyInternal = configuration["ApiKeyInternal"]!;
    public async Task<BaseMessageResponse<object>> DeactiveUser(DeactiveUserStateCommand request, CancellationToken cancellationToken)
    {
        if (request.MobileNumber != request.RequestInfo.MobileNumber)
            return MessageResponse.GenerateMessageResponse(EResponseStatusCode.WrongParameters);

        var activationCodeModel = await GenerateAndStoreActivationCodeAsync(request.RequestInfo.MobileNumber, cancellationToken);
        return await SendOtpAsync(activationCodeModel, cancellationToken);
    }
    public async Task<BaseMessageResponse<object>> VerifyOtp(VerifyOtpDeactiveUserStateCommand request, CancellationToken cancellationToken)
    {
        var verificationFailure = await CheckOtpCodeVerificationFailureAsync(request, cancellationToken);
        if (verificationFailure is not null)
        {
            await AddChangeUserActiveState(request, false, cancellationToken);
            return verificationFailure;
        }
        var user = await userRepository.FindById(request.CurrentUser.Id, cancellationToken);
        if (user == null)
        {
            return MessageResponse.GenerateMessageResponse<object>(EResponseStatusCode.NotFound)!;
        }
        user.Deactive();
        await userRepository.UpdateAsync(user, cancellationToken);

        await SignOut(request, cancellationToken);
        await AddChangeUserActiveState(request, true, cancellationToken);
        return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully)!;
    }

    #region [ - Private Methods - ]
    private async Task<BaseMessageResponse<object>?> CheckOtpCodeVerificationFailureAsync(VerifyOtpDeactiveUserStateCommand request, CancellationToken cancellationToken)
    {
        var activationCode = new ActivationCodeModel(request.RequestInfo.MobileNumber, request.OtpCode);

        activationCode.SetTime(DateTime.Now);
        activationCode.Type = Enums.ActivationCode.ChangeUserActiveState;

        var status = await activeTokenQueryRepository.VerifyActivationCodeAsync(activationCode, cancellationToken);
        await activeTokenCommandRepository.DeactivateActivationCodeAsync(activationCode, cancellationToken);

        if (!status)
        {
            return MessageResponse.GenerateMessageResponse<object>(EResponseStatusCode.WrongParameters)!;
        }

        return null;
    }
    private async Task AddChangeUserActiveState(VerifyOtpDeactiveUserStateCommand request, bool isSuccess, CancellationToken cancellationToken)
    {
        var changeUserActiveStateHistory = new ChangeUserActiveStateHistory()
        {
            IsSuccess = isSuccess,
            UserId = request.CurrentUser.Id,
            CreatedAt = DateTime.Now,
        };

        await changeUserActiveStateRepository.AddAsync(changeUserActiveStateHistory, cancellationToken);
        await changeUserActiveStateRepository.SaveChangeAsync(cancellationToken);
    }
    private async Task<BaseMessageResponse<object>> SendOtpAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken)
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
        return MessageResponse.GenerateMessageResponse<object>(responseStatus)!;
    }
    private async Task<ActivationCodeModel> GenerateAndStoreActivationCodeAsync(string mobileNumber, CancellationToken cancellationToken)
    {
        var activationCode = new ActivationCodeModel(mobileNumber)
        {
            Type = Enums.ActivationCode.ChangeUserActiveState
        };

        activationCode.GenerateCode(false);
        activationCode.SetTime(DateTime.Now);

        await activeTokenCommandRepository.DeactivateActivationCodeByMobileNumber(mobileNumber, cancellationToken);
        await activeTokenCommandRepository.InsertActivationCodeAsync(activationCode, cancellationToken);
        return activationCode;
    }
    private async Task<BaseMessageResponse<object>> SignOut(VerifyOtpDeactiveUserStateCommand request, CancellationToken cancellationToken)
    {
        await activeTokenRepository.DeActiveTokenAsync(request.RequestInfo.MobileNumber,
            request.RequestInfo.Token, cancellationToken);
        UserModel currentPerson =
            await userRepository.GetByMobileNumberAsync(request.RequestInfo.MobileNumber, cancellationToken);
        await userRepository.InquiryFireBaseToken(currentPerson.Id, cancellationToken);

        redisCacheManager.Remove(string.Format(CacheKey.TosanUserSessionId, request.RequestInfo.Token));

        return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully);
    }
    #endregion
}