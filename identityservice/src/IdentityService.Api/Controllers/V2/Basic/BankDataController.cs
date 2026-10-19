using IdentityService.Application.Handler.Basic;

namespace IdentityService.Api.Controllers.V2.Basic;

[ApiVersion("2")]
public class BankDataController : BaseIdentityApplicationApi
{
    [HttpGet]
    public async Task<IActionResult> BankData([FromQuery] GetBankDataRequest query, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(query, cancellationToken));
}