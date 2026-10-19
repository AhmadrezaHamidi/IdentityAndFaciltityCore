using IdentityService.Domain.Entity;

namespace IdentityService.Application.Handler.User;

public class InternalCheckUserManagmentHandler(IUserManagementRepository repository) : IBaseHandler<InternalCheckUserManagmentRequest, BaseMessageResponse<bool>>
{
    public async Task<BaseMessageResponse<bool>> Handle(InternalCheckUserManagmentRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully, new());

        var userManagment = await repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (userManagment is null)
        {
            response.Data = false;
            return response;
        }
        if ((userManagment.TosanRegister is null) || userManagment.TosanRegister.Value == false)
        {
            response.Data = true;
            return response;
        }
        response.Data = false;
        return response;
    }
}
public record InternalCheckUserManagmentRequest(
    int UserId
) : BaseCommand<BaseMessageResponse<bool>>;
