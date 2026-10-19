using IdentityService.Application.Handler.User;

namespace IdentityService.Api.Controllers.V2.User;

[ApiVersion("2")]
public class InternalUserController : BaseApplicationApi
{
    [HttpGet]
    [InternalApi]
    public async Task<IActionResult> UserInfo([FromQuery] GetInternalUserInfoRequest query, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(query, cancellationToken));

    [HttpPost]
    [InternalApi]
    public async Task<IActionResult> UserBriefInfo([FromBody] GetInternalUserBriefInfoRequest query, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(query, cancellationToken));

    [HttpPost]
    [InternalApi]
    public async Task<IActionResult> UpdateCustomerNumber([FromBody] InternalUpdateCustomerNumberRequest command, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(command, cancellationToken));

    [HttpPost]
    [InternalApi]
    public async Task<IActionResult> GeneratePersonOpenAccount([FromBody] GeneratePersonOpenAccountRequest command, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(command, cancellationToken));

    [HttpGet]
    [InternalApi]
    public async Task<IActionResult> GetUserByMobileNumber([FromQuery] InternalGetUserByMobileNumberRequest command, CancellationToken cancellationToken)
    => BaseResult(await _mediator.Send(command, cancellationToken));

    [HttpGet]
    [InternalApi]
    public async Task<IActionResult> GetPersonByNationalCode([FromQuery] InternalGetUserByNationalCodeRequest command, CancellationToken cancellationToken)
    => BaseResult(await _mediator.Send(command, cancellationToken));
}
