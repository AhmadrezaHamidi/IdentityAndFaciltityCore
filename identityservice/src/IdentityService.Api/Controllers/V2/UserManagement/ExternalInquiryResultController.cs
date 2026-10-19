
using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;


namespace IdentityService.Api.Controllers.UserManagementKycLog;

[ApiVersion("2")]
public class ExternalInquiryResultController : BaseIdentityApplicationApi
{
    [HttpPost]
    [ExternalApi]
    public async Task<IActionResult> UpdateUM([FromBody] ExternalUpdateUMRequest request, CancellationToken cancellationToken)
    {
        return BaseResult(await _mediator.Send(request, cancellationToken));
    }
    
    [HttpPost]
    [ExternalApi]
    public async Task<IActionResult> VinaraUpdateUm([FromBody] VinaraPushApiVerificationRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(request, cancellationToken));
    }
    
    [HttpPost]
    [ExternalApi]
    public async Task<IActionResult> RetrieveUserInfo([FromBody] VinaraRetrieveUserInformationRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(request, cancellationToken));
    }
}
  