using IdentityService.Application.Handler.Identities.Services;
using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.VerifyUsernameChangeOtp;

public class VerifyUsernameChangeOtpCommandHandler(IIdentityService identityService) : IBaseHandler<VerifyUsernameChangeOtpCommand, BaseMessageResponse<VerifyUsernameChangeOtpDto>>
{
    public async Task<BaseMessageResponse<VerifyUsernameChangeOtpDto>> Handle(VerifyUsernameChangeOtpCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.VerifyUsernameChangeOtp(request, cancellationToken);
        return result;
    }
}