using IdentityService.Application.Handler.Identities.Services.ChangeUserActiveState;

namespace IdentityService.Application.Handler.Identities.Commands.ChangeUserActiveState.VerifyChangeUserStateOtp;

public class VerifyOtpDeactiveUserStateCommandHandler(IChangeUserActiveStateService changeUserActiveStateService) : IBaseHandler<VerifyOtpDeactiveUserStateCommand, BaseMessageResponse<object>>
{
    public async Task<BaseMessageResponse<object>> Handle(VerifyOtpDeactiveUserStateCommand request, CancellationToken cancellationToken)
    {
        var response = await changeUserActiveStateService.VerifyOtp(request, cancellationToken);
        return response;
    }
}
