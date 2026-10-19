using IdentityService.Application.Handler.Identities.Commands.InternalActivationCode.Create;
using IdentityService.Application.Handler.Identities.Commands.InternalActivationCode.VerifyOtp;

namespace IdentityService.Api.Controllers.V2;

[ApiVersion("2")]
public class InternalActivationCodeController : BaseIdentityApplicationApi
{
    [HttpPost]
    [InternalApi]
    public async Task<IActionResult> Create(CreateActivationCodeCommand command, CancellationToken cancellationToken)
    {
        return BaseResult(await _mediator.Send(command, cancellationToken));
    }
    [HttpPost]
    [InternalApi]
    public async Task<IActionResult> Verify(VerifyActivationCodeCommand command, CancellationToken cancellationToken)
    {
        return BaseResult(await _mediator.Send(command, cancellationToken));
    }
}
