using IdentityService.Application.Handler.Identities.Services;
using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.ChangeUsername;

public class ChangeUsernameCommandHandler(IIdentityService identityService) : IBaseHandler<ChangeUsernameCommand, BaseMessageResponse<ChangeUsernameDto>>
{
    public async Task<BaseMessageResponse<ChangeUsernameDto>> Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.ChangeUsername(request, cancellationToken);
        return result;
    }
}