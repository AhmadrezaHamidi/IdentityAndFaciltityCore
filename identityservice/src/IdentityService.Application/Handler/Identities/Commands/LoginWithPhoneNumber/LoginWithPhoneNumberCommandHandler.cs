using IdentityService.Application.Handler.Identities.Services;
using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.LoginWithPhoneNumber;

public class LoginWithPhoneNumberCommandHandler(IIdentityService identityService) : IBaseHandler<LoginWithPhoneNumberCommand, BaseMessageResponse<LoginWithPhoneNumberDto>>
{
    public async Task<BaseMessageResponse<LoginWithPhoneNumberDto>> Handle(LoginWithPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var response = await identityService.LoginWithPhoneNumber(request, cancellationToken);
        return response;
    }
}