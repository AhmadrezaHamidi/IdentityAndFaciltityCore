using IdentityService.Application.Handler.Identities.Services;
using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.VerifyLoginOtp;

public class VerifyLoginOtpCommandHandler(IIdentityService identityService) : IBaseHandler<VerifyLoginOtpCommand, BaseMessageResponse<VerifyLoginOtpDto>>
{
    public async Task<BaseMessageResponse<VerifyLoginOtpDto>> Handle(VerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        var response = await identityService.VerifyLoginOtp(request, cancellationToken);
        return response;
    }
}