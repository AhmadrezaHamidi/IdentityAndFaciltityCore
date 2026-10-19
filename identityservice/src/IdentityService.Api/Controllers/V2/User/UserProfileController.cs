using IdentityService.Application.Handler.User;

namespace IdentityService.Api.Controllers.V2.User;


[ApiVersion("2")]
public class UserProfileController : BaseIdentityApplicationApi
{
    [HttpPut]
    public async Task<IActionResult> AutoModifyProfile([FromQuery] AutoModifyUserProfileRequest command, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(command, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> ModifyProfile([FromBody] ModifyUserProfileRequest command, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(command, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> GetSejamStatus([FromQuery] GetSejamStatusRequest command, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(command, cancellationToken));

}