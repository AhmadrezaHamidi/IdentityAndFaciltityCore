using IdentityService.Application.Handler.Identities.Services;
using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.RegisterUser;

public class RegisterUserCommandHandler(IIdentityService identityService) : IBaseHandler<RegisterUserCommand, BaseMessageResponse<RegisterUserDto>>
{
    public async Task<BaseMessageResponse<RegisterUserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var response = await identityService.RegisterUser(request, cancellationToken);
        return response;
    }
}