using IdentityService.Application.Handler.User;

namespace IdentityService.Application.Services.ChangeMobileNumber;

public interface IChangeMobileNumberService
{
    Task<BaseMessageResponse<MakeChangeMobileNumberResponse>> InsertChangeMobileNumberRequest(MakeChangeMobileNumberRequest request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<MakeChangeMobileNumberResponse>> CheckAndChangeInactiveMobileNumberRequest(MakeChangeMobileNumberRequest request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<object>> Change(string trackId, string referenceCode, CancellationToken cancellationToken);
    Task<BaseMessageResponse<object>> Reverse(string trackId, CancellationToken cancellationToken);
    Task<BaseMessageResponse<CheckChangeMobileNumberPaymentResponse>> CheckPayment(string trackId, CancellationToken cancellationToken);
    Task<BaseMessageResponse<ChangeMobileNumberResultResponse>> RetrieveResult(string trackId, CancellationToken cancellationToken);
    Task<BaseMessageResponse<object>> SendOtp(ChangeMobileNumberSendOtpRequest request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<ChangeMobileNumberPaymentResponse>> VerifyOtp(ChangeMobileNumberVerifyOtpRequest request, CancellationToken cancellationToken);
}