using IdentityService.Application.Services.ChangeMobileNumber;

namespace IdentityService.Application.Handler.User;

public class MakeChangeMobileNumberRequestHandler(IChangeMobileNumberService changeMobileNumberService) : IBaseHandler<MakeChangeMobileNumberRequest, BaseMessageResponse<MakeChangeMobileNumberResponse>>
{
    public async Task<BaseMessageResponse<MakeChangeMobileNumberResponse>> Handle(MakeChangeMobileNumberRequest request, CancellationToken cancellationToken)
    {
        var response = await changeMobileNumberService.InsertChangeMobileNumberRequest(request, cancellationToken);
        return response;
    }
}

public record MakeChangeMobileNumberRequest(string OldMobileNumber, string NewMobileNumber, string NationalId) : BaseCommand<BaseMessageResponse<MakeChangeMobileNumberResponse>>;

public record MakeChangeMobileNumberResponse(string TrackId)
{
    public bool IsChangeDone = false;
}