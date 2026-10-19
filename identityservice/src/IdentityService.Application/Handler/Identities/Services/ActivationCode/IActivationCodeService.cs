using IdentityService.Application.Handler.Identities.Commands.InternalActivationCode.Create;
using IdentityService.Application.Handler.Identities.Commands.InternalActivationCode.VerifyOtp;

namespace IdentityService.Application.Handler.Identities.Services.ActivationCode;

public interface IActivationCodeService
{
    Task<CreateActivationCodeResponse> Create(CreateActivationCodeCommand command, CancellationToken cancellationToken);
    Task<bool> Verify(VerifyActivationCodeCommand request, CancellationToken cancellationToken);
}
