using IdentityService.Application.Services.ChangeMobileNumber;

namespace IdentityService.Application.Handler.User;

public class ReverseChangeMobileNumberHandler(IChangeMobileNumberService changeMobileNumberService) : IBaseHandler<ReverseChangeMobileNumberRequest, BaseMessageResponse<object>>
{
    public async Task<BaseMessageResponse<object>> Handle(ReverseChangeMobileNumberRequest request, CancellationToken cancellationToken)
    {
        var response = await changeMobileNumberService.Reverse(request.TrackId, cancellationToken);
        return response;
    }
}

public record ReverseChangeMobileNumberRequest(string TrackId) : BaseCommand<BaseMessageResponse<object>>;
