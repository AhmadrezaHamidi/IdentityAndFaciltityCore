using IdentityService.Application.Handler.User;

namespace IdentityService.Api.Controllers.V2.User
{
    [ApiVersion("2")]
    public class InternalUserProfileController : BaseApplicationApi
    {
        
        [HttpPost]
        [InternalApi]
        public async Task<IActionResult> AddUserProfileInfo([FromBody] AddUserProfileInfoRequest command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(command, cancellationToken));

        [HttpPost]
        [InternalApi]
        public async Task<IActionResult> UserProfileInfo([FromBody] InternalUserProfileInfoRequest command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(command));

        [HttpPost]
        [InternalApi]
        public async Task<IActionResult> UpdateSejamStatus([FromBody] InternalUpdateSejamStatusRequest command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(command, cancellationToken));
        
        
        [HttpPost]
        [InternalApi]
        public async Task<IActionResult> Update([FromBody] InternalUpdateUserProfileRequest command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(command));

    }
}
