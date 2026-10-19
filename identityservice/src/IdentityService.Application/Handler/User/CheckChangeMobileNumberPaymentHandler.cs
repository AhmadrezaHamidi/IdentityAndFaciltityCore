using IdentityService.Application.Services.ChangeMobileNumber;

namespace IdentityService.Application.Handler.User;

public class CheckChangeMobileNumberPaymentHandler(IChangeMobileNumberService changeMobileNumberService) : IBaseHandler<CheckChangeMobileNumberPaymentRequest, BaseMessageResponse<CheckChangeMobileNumberPaymentResponse>>
{
    public async Task<BaseMessageResponse<CheckChangeMobileNumberPaymentResponse>> Handle(CheckChangeMobileNumberPaymentRequest request, CancellationToken cancellationToken)
    {
        var response = await changeMobileNumberService.CheckPayment(request.TrackId, cancellationToken);
        return response;
    }
}

public record CheckChangeMobileNumberPaymentRequest(string TrackId) : BaseCommand<BaseMessageResponse<CheckChangeMobileNumberPaymentResponse>>;

public record CheckChangeMobileNumberPaymentResponse(long Amount, bool IsPaid);