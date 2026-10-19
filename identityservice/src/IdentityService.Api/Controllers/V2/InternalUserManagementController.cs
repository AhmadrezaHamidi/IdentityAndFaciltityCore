using IdentityService.Application.Handler.User;
using IdentityService.Application.Handler.UserManagement;

namespace IdentityService.Api.Controllers.V2;

[ApiVersion("2")]
public class InternalUserManagementController : BaseIdentityApplicationApi
{
    [HttpPost]
    [InternalApi]
    public async Task<IActionResult> CheckUMToken([FromBody] InternalCheckUMTokenRequest request, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(request, cancellationToken));

    [HttpGet]
    [InternalApi]
    public async Task<IActionResult> UmToken([FromQuery] UMTokenRequest request, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(request, cancellationToken));

    [HttpPost]
    [InternalApi]
    public async Task<IActionResult> CheckUserManagment([FromBody] InternalCheckUserManagmentRequest request, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(request, cancellationToken));

}