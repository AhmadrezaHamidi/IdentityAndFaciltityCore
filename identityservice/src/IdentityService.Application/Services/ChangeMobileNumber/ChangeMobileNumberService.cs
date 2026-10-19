using IdentityService.Application.Handler.User;
using IdentityService.Contracts.Admin.Repositories.ActiveTokens;
using IdentityService.Contracts.ChangeMobileNumber.Repositories;
using IdentityService.Domain.Entity.ChangeMobileNumber;
using IdentityService.Domain.IRepositories;
using IdentityService.Domain.Models;
using IdentityService.Services.IServices;
using IdentityService.Services.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OmidBank.Common.InternalThirdParty.BankService.Request;
using OmidBank.Common.InternalThirdParty.ShahkarService.Request;
using OmidBank.Common.InternalThirdParty.ShahkarService;
using OmidBank.Common.Cache;
using static OmidBank.Common.InternalThirdParty.ShahkarService.ShahkarServiceProvider;

namespace IdentityService.Application.Services.ChangeMobileNumber;

public class ChangeMobileNumberService(
    IChangeMobileNumberRepository changeMobileNumberRepository,
    IActiveTokenCommandRepository activeTokenCommandRepository,
    IActiveTokenQueryRepository activeTokenQueryRepository,
    IUserRepository userRepository,
    ISmsService smsService,
    IConfiguration configuration,
    IOptions<ChangeMobileNumberOptions> options,
    IOptions<ShahkarProviderOptions> shahkarOptions, 
    IRedisCacheManager redisCacheManager, HttpClient httpClient) : IChangeMobileNumberService
{
    private readonly string _apiKeyInternal = configuration["ApiKeyInternal"]!;

    public async Task<BaseMessageResponse<MakeChangeMobileNumberResponse>> InsertChangeMobileNumberRequest(MakeChangeMobileNumberRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<MakeChangeMobileNumberResponse>(EResponseStatusCode.DoneSuccessfully)!;
        if (request.NewMobileNumber == request.OldMobileNumber)
        {
            response = MessageResponse.GenerateMessageResponse<MakeChangeMobileNumberResponse>(EResponseStatusCode.BadRequest)!;
            response.Message = "شماره جدید و قدیم نمی توانند یکسان باشند";
            return response!;
        }

        var user = await userRepository.GetByMobileNumberAsync(request.OldMobileNumber, cancellationToken);
        if (user is null)
        {
            response = MessageResponse.GenerateMessageResponse<MakeChangeMobileNumberResponse>(EResponseStatusCode.BadRequest)!;
            response.Message = "کاربری با این شماره همراه وجود ندارد.";
            return response!;
        }

        var entity = MakeChangeMobileNumberEntity(request, user.Id);
        await changeMobileNumberRepository.InsertAsync(entity, cancellationToken);
        response.Data = new MakeChangeMobileNumberResponse(entity.TrackId);
        if (entity.WageAmount != 0) 
            return response!;
        
        var changeResponse = await Change(entity, "", cancellationToken);
        if (!changeResponse.IsSuccess)
            return MessageResponse.GenerateMessageResponse<MakeChangeMobileNumberResponse>(EResponseStatusCode.Failed)!;
        response.Data.IsChangeDone = true;
        return response!;
    }
    
    public async Task<BaseMessageResponse<MakeChangeMobileNumberResponse>> CheckAndChangeInactiveMobileNumberRequest(MakeChangeMobileNumberRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<MakeChangeMobileNumberResponse>(EResponseStatusCode.DoneSuccessfully)!;
        var user = await userRepository.GetByMobileNumberAsync(request.OldMobileNumber, cancellationToken);
        if (user is null)
        {
            response = MessageResponse.GenerateMessageResponse<MakeChangeMobileNumberResponse>(EResponseStatusCode.Unauthorized)!;
            response.Message = "کاربری با این شماره همراه وجود ندارد.";
            return response!;
        }
        if (!user.MobileNumber.StartsWith("00"))
            return MessageResponse.GenerateMessageResponse<MakeChangeMobileNumberResponse>(EResponseStatusCode.BadRequest)!;

        var entity = MakeChangeMobileNumberEntity(request, user.Id);
        await changeMobileNumberRepository.InsertAsync(entity, cancellationToken);
        response.Data = new MakeChangeMobileNumberResponse(entity.TrackId);
        entity.WageAmount = 0;
        var changeResponse = await Change(entity, "", cancellationToken);
        if (!changeResponse.IsSuccess)
            return MessageResponse.GenerateMessageResponse<MakeChangeMobileNumberResponse>(EResponseStatusCode.Failed)!;
        response.Data.IsChangeDone = true;
        return response!;
    }

    private ChangeMobileNumberEntity MakeChangeMobileNumberEntity(MakeChangeMobileNumberRequest request, int userId)
    {
        return new ChangeMobileNumberEntity
        {
            Status = EChangeMobileNumberStatus.Unknown,
            NationalId = request.NationalId,
            UserId = userId,
            OldMobileNumber = request.OldMobileNumber,
            NewMobileNumber = request.NewMobileNumber,
            TrackId = Guid.NewGuid().ToString(),
            IsLoggedIn = request.CurrentUser is not null,
            WageAmount = options.Value.EnableWage ? options.Value.Wage : 0,
            ReferenceCode = string.Empty,
            CreatedAt = DateTime.Now,
        };
    }

    public async Task<BaseMessageResponse<object>> Change(string trackId, string referenceCode, CancellationToken cancellationToken)
    {
        var entity = await changeMobileNumberRepository.RetrieveByTrackIdAsync(trackId, cancellationToken);
        return await Change(entity, referenceCode, cancellationToken);
    }

    private async Task<BaseMessageResponse<object>> Change(ChangeMobileNumberEntity? entity, string referenceCode, CancellationToken cancellationToken)
    {
        if (entity is null)
            return MessageResponse.GenerateMessageResponse(EResponseStatusCode.ItemIsNotFound)!;
        var result = await changeMobileNumberRepository.ChangeMobileNumbers(entity.OldMobileNumber, entity.NewMobileNumber, entity.NationalId, referenceCode, EChangeMobileNumberStatus.Success, cancellationToken);
        return MessageResponse.GenerateMessageResponse(result ? EResponseStatusCode.DoneSuccessfully : EResponseStatusCode.Failed)!;
    }

    public async Task<BaseMessageResponse<object>> Reverse(string trackId, CancellationToken cancellationToken)
    {
        var entity = await changeMobileNumberRepository.RetrieveByTrackIdAsync(trackId, cancellationToken);
        if (entity is null)
            return MessageResponse.GenerateMessageResponse(EResponseStatusCode.ItemIsNotFound)!;
        var result = await changeMobileNumberRepository.ChangeMobileNumbers(entity.NewMobileNumber, entity.OldMobileNumber, entity.NationalId, entity.ReferenceCode, EChangeMobileNumberStatus.PaymentFailed, cancellationToken);
        return MessageResponse.GenerateMessageResponse(result ? EResponseStatusCode.DoneSuccessfully : EResponseStatusCode.Failed)!;
    }

    public async Task<BaseMessageResponse<CheckChangeMobileNumberPaymentResponse>> CheckPayment(string trackId, CancellationToken cancellationToken)
    {
        var entity = await changeMobileNumberRepository.RetrieveByTrackIdAsync(trackId, cancellationToken);
        if (entity is null)
            return MessageResponse.GenerateMessageResponse<CheckChangeMobileNumberPaymentResponse>(EResponseStatusCode.ItemIsNotFound)!;
        var response = MessageResponse.GenerateMessageResponse<CheckChangeMobileNumberPaymentResponse>(EResponseStatusCode.DoneSuccessfully)!;
        response.Data = new CheckChangeMobileNumberPaymentResponse(entity.WageAmount, entity.Status is not EChangeMobileNumberStatus.Unknown);
        return response!;
    }

    public async Task<BaseMessageResponse<ChangeMobileNumberResultResponse>> RetrieveResult(string trackId, CancellationToken cancellationToken)
    {
        var entity = await changeMobileNumberRepository.RetrieveByTrackIdAsync(trackId, cancellationToken);
        if (entity is null)
            return MessageResponse.GenerateMessageResponse<ChangeMobileNumberResultResponse>(EResponseStatusCode.ItemIsNotFound)!;
        var response = MessageResponse.GenerateMessageResponse<ChangeMobileNumberResultResponse>(EResponseStatusCode.DoneSuccessfully)!;
        var state = entity.Status == EChangeMobileNumberStatus.Success;
        response.Data = new ChangeMobileNumberResultResponse(state ? 1 : 2, state ? entity.NewMobileNumber : string.Empty);
        return response!;
    }

    public async Task<BaseMessageResponse<object>> SendOtp(ChangeMobileNumberSendOtpRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByMobileNumberAsync(request.OldMobileNumber, cancellationToken);
        var shahkarRequest = new InquiryShahkarRequest(request.NationalId, request.NewMobileNumber, user.Id.ToString());
        ShahkarServiceProvider shahkarServiceProvider = new ShahkarServiceProvider(configuration, redisCacheManager, shahkarOptions, httpClient);
        var checkIsMached = await shahkarServiceProvider.ValidateIdentityByShahkar(new ShahkarRequestViewModel()
        {
            MobileNumber = request.NewMobileNumber,//_baseRequest.RequestInfo.MobileNumber,
            NationalId = request.NationalId//personTemp.NationalCode,
        }, cancellationToken) ;
        if (checkIsMached != null)
        {
            if (checkIsMached.Data.response != 200)
                return MessageResponse.GenerateMessageResponse(EResponseStatusCode.ShahkarError);
        }
        else
            return MessageResponse.GenerateMessageResponse(EResponseStatusCode.ShahkarServiceIsNotAvailable);

        var activationCode = await GenerateAndStoreActivationCodeAsync(request, cancellationToken);
        return await SendOtpAsync(activationCode, cancellationToken);
    }

private async Task<ActivationCodeModel> GenerateAndStoreActivationCodeAsync(ChangeMobileNumberSendOtpRequest request, CancellationToken cancellationToken)
{
    var activationCode = new ActivationCodeModel(request.NewMobileNumber)
    {
        Type = Enums.ActivationCode.ChangeMobileNumber,
    };

    activationCode.GenerateCode(false);
    activationCode.SetTime(DateTime.Now);

    await activeTokenCommandRepository.DeactivateActivationCodeByMobileNumber(request.NewMobileNumber, cancellationToken);
    await activeTokenCommandRepository.InsertActivationCodeAsync(activationCode, cancellationToken);
    return activationCode;
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

public async Task<BaseMessageResponse<ChangeMobileNumberPaymentResponse>> VerifyOtp(ChangeMobileNumberVerifyOtpRequest request, CancellationToken cancellationToken)
{
    var response = await CheckOtpCodeVerificationAsync(request, cancellationToken);
    if (!response.IsSuccess)
        return response;
    response = MessageResponse.GenerateMessageResponse<ChangeMobileNumberPaymentResponse>(EResponseStatusCode.DoneSuccessfully)!;
    response.Data = options.Value.EnableWage ?
        new ChangeMobileNumberPaymentResponse(options.Value.Wage, options.Value.Wage) :
        new ChangeMobileNumberPaymentResponse(0, 0);
    return response;
}

private async Task<BaseMessageResponse<ChangeMobileNumberPaymentResponse>> CheckOtpCodeVerificationAsync(ChangeMobileNumberVerifyOtpRequest request, CancellationToken cancellationToken)
{
    var activationCode = new ActivationCodeModel(request.NewMobileNumber, request.OtpCode);
    activationCode.SetTime(DateTime.Now);

    var status = await activeTokenQueryRepository.VerifyActivationCodeAsync(activationCode, cancellationToken);
    await activeTokenCommandRepository.DeactivateActivationCodeAsync(activationCode, cancellationToken);
    return MessageResponse.GenerateMessageResponse<ChangeMobileNumberPaymentResponse>(status ? EResponseStatusCode.DoneSuccessfully : EResponseStatusCode.WrongParameters)!;
}
}
