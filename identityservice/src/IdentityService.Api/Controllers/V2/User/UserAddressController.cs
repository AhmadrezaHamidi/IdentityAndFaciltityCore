using IdentityService.Application.Handler.User;

namespace IdentityService.Api.Controllers.V2.User;


[ApiVersion("2")]
public class UserAddressController : BaseIdentityApplicationApi
{
    [HttpPost]
    public async Task<IActionResult> AutoModifyAddress([FromBody] AutoModifyUserAddressRequest command, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(command, cancellationToken));
    
    [HttpPost]
    public async Task<IActionResult> ModifyAddress([FromBody] ModifyUserAddressRequest command, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(command, cancellationToken));
}