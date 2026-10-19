using IdentityService.Application.Handler.Identities.Services;
using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.VerifyPasswordChangeOtp;

public class VerifyPasswordChangeOtpCommandHandler(IIdentityService identityService) : IBaseHandler<VerifyPasswordChangeOtpCommand, BaseMessageResponse<VerifyPasswordChangeOtpDto>>
{
    public async Task<BaseMessageResponse<VerifyPasswordChangeOtpDto>> Handle(VerifyPasswordChangeOtpCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.VerifyPasswordChangeOtp(request, cancellationToken);
        return result;
    }
}