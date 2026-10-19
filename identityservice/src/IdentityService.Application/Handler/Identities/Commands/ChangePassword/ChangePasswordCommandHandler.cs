using IdentityService.Application.Handler.Identities.Services;
using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.ChangePassword;

public class ChangePasswordCommandHandler(IIdentityService identityService) : IBaseHandler<ChangePasswordCommand, BaseMessageResponse<ChangePasswordDto>>
{
    public async Task<BaseMessageResponse<ChangePasswordDto>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.ChangePassword(request, cancellationToken);
        return result;
    }
}