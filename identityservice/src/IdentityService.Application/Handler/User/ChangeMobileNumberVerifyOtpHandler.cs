using IdentityService.Application.Services.ChangeMobileNumber;

namespace IdentityService.Application.Handler.User;

public class ChangeMobileNumberVerifyOtpHandler(IChangeMobileNumberService changeMobileNumberService) : IBaseHandler<ChangeMobileNumberVerifyOtpRequest, BaseMessageResponse<ChangeMobileNumberPaymentResponse>>
{
    public async Task<BaseMessageResponse<ChangeMobileNumberPaymentResponse>> Handle(ChangeMobileNumberVerifyOtpRequest request, CancellationToken cancellationToken)
    {
        var response = await changeMobileNumberService.VerifyOtp(request, cancellationToken);
        return response;
    }
}

public record ChangeMobileNumberVerifyOtpRequest(string OtpCode, string NewMobileNumber) : BaseCommand<BaseMessageResponse<ChangeMobileNumberPaymentResponse>>;

public record ChangeMobileNumberPaymentResponse(long Wage, long TotalPayment);