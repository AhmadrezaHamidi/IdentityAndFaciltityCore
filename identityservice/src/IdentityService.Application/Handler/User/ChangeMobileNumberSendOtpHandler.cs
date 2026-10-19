using IdentityService.Application.Services.ChangeMobileNumber;

namespace IdentityService.Application.Handler.User;

public class ChangeMobileNumberSendOtpHandler(IChangeMobileNumberService changeMobileNumberService) : IBaseHandler<ChangeMobileNumberSendOtpRequest, BaseMessageResponse<object>>
{
    public async Task<BaseMessageResponse<object>> Handle(ChangeMobileNumberSendOtpRequest request, CancellationToken cancellationToken)
    {
        var response = await changeMobileNumberService.SendOtp(request, cancellationToken);
        return response;
    }
}

public record ChangeMobileNumberSendOtpRequest(string OldMobileNumber, string NewMobileNumber, string NationalId) : BaseCommand<BaseMessageResponse<object>>;
