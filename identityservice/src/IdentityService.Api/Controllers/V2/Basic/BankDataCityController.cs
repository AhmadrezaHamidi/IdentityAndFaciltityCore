using IdentityService.Application.Handler.Basic;

namespace IdentityService.Api.Controllers.V2.Basic;

[ApiVersion("2")]
public class BankDataCityController : BaseIdentityApplicationApi
{
    [HttpGet]
    public async Task<IActionResult> BankDataCity([FromQuery] GetBankDataCityRequest query, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(query, cancellationToken));
}