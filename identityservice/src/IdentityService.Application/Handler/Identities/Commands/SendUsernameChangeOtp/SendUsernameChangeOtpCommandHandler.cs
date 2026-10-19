using IdentityService.Application.Handler.Identities.Services;
using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.SendUsernameChangeOtp;

public class SendUsernameChangeOtpCommandHandler(IIdentityService identityService) : IBaseHandler<SendUsernameChangeOtpCommand, BaseMessageResponse<SendUsernameChangeOtpDto>>
{
    public async Task<BaseMessageResponse<SendUsernameChangeOtpDto>> Handle(SendUsernameChangeOtpCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.SendUsernameChangeOtp(request, cancellationToken);
        return result;
    }
}