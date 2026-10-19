using IdentityService.Application.Services.ChangeMobileNumber;

namespace IdentityService.Application.Handler.User;

public class ChangeMobileNumberHandler(IChangeMobileNumberService changeMobileNumberService) : IBaseHandler<ChangeMobileNumberRequest, BaseMessageResponse<object>>
{
    public async Task<BaseMessageResponse<object>> Handle(ChangeMobileNumberRequest request, CancellationToken cancellationToken)
    {
        var response = await changeMobileNumberService.Change(request.TrackId, request.ReferenceCode, cancellationToken);
        return response;
    }
}

public record ChangeMobileNumberRequest(string TrackId, string ReferenceCode) : BaseCommand<BaseMessageResponse<object>>;
