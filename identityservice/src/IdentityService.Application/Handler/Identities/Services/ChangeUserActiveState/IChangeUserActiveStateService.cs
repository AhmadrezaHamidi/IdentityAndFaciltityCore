using IdentityService.Application.Handler.Identities.Commands.ChangeUserActiveState.DeactiveUserState;
using IdentityService.Application.Handler.Identities.Commands.ChangeUserActiveState.VerifyChangeUserStateOtp;

namespace IdentityService.Application.Handler.Identities.Services.ChangeUserActiveState;

public interface IChangeUserActiveStateService
{
    Task<BaseMessageResponse<object>> DeactiveUser(DeactiveUserStateCommand request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<object>> VerifyOtp(VerifyOtpDeactiveUserStateCommand request, CancellationToken cancellationToken);
}
