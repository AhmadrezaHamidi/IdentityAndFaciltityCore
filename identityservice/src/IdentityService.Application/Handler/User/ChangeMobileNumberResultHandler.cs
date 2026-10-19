using IdentityService.Application.Services.ChangeMobileNumber;

namespace IdentityService.Application.Handler.User;

public class ChangeMobileNumberResultHandler(IChangeMobileNumberService changeMobileNumberService) : IBaseHandler<ChangeMobileNumberResultRequest, BaseMessageResponse<ChangeMobileNumberResultResponse>>
{
    public async Task<BaseMessageResponse<ChangeMobileNumberResultResponse>> Handle(ChangeMobileNumberResultRequest request, CancellationToken cancellationToken)
    {
        var response = await changeMobileNumberService.RetrieveResult(request.TrackId, cancellationToken);
        return response;
    }
}

public record ChangeMobileNumberResultRequest(string TrackId) : BaseCommand<BaseMessageResponse<ChangeMobileNumberResultResponse>>;

public record ChangeMobileNumberResultResponse(int Status, string MobileNumber);