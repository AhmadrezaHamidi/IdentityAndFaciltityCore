using IdentityService.Application.Handler.Basic;

namespace IdentityService.Api.Controllers.V2.Basic;

[ApiVersion("2")]
public class BankDataProvinceController : BaseIdentityApplicationApi
{
    [HttpGet]
    public async Task<IActionResult> BankDataProvince([FromQuery] GetBankDataProvinceRequest query, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(query, cancellationToken));
}
