using IdentityService.Application.Handler.Identities.Services;
using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.SendPasswordChangeOtp;

public class SendPasswordChangeOtpCommandHandler(IIdentityService identityService) : IBaseHandler<SendPasswordChangeOtpCommand, BaseMessageResponse<SendPasswordChangeOtpDto>>
{
    public async Task<BaseMessageResponse<SendPasswordChangeOtpDto>> Handle(SendPasswordChangeOtpCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.SendPasswordChangeOtp(request, cancellationToken);
        return result;
    }
}