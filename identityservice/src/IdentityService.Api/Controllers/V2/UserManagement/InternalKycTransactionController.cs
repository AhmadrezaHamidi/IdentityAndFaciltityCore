using IdentityService.Application.Handler.UserManagement;

namespace IdentityService.Api.Controllers.V2.UserManagement;

[ApiVersion("2")]
public class InternalKycTransactionController : BaseApplicationApi
{
    [HttpPost]
    [InternalApi]
    public async Task<IActionResult> DeliverKycUmRegister([FromBody] DeliverKycUmServiceRequest query, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(query, cancellationToken));
    
    [HttpPost]
    [InternalApi]
    public async Task<IActionResult> RetrieveKycUmTransactionLog([FromBody]RetrieveKycUmTransactionLogRequest request, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(request, cancellationToken));
}