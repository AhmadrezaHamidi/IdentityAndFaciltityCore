//using IdentityService.Application.Handler.Admin.CustomerManagement;
using OmidBank.Common.Attribute;

namespace Admin.Api.Controllers.V2.CustomerManagement
{
    //[Route("CustomerManagement")]
    //[ApiVersion("2")]
    //public class CustomerManagementController : BaseIdentityApplicationApi
    //{
    //    [HttpGet("Customers")]
    //    [AuthenticationUserAdminByCookie]
    //    public async Task<IActionResult> SearchCustomers([FromQuery] string? nationalCode, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
    //        => BaseResult(await _mediator.Send(new SearchCustomersQuery(nationalCode, page, pageSize), cancellationToken));

    //    [HttpPut("Customers/{id:int}/BlockStatus")]
    //    [AuthenticationUserAdminByCookie]
    //    public async Task<IActionResult> SetBlockStatus(int id, [FromQuery] bool blocked, CancellationToken cancellationToken)
    //        => BaseResult(await _mediator.Send(new SetCustomerBlockStatusCommand(id, blocked), cancellationToken));

    //    [HttpGet("Customers/{id:int}/ServiceAccess")]
    //    [AuthenticationUserAdminByCookie]
    //    public async Task<IActionResult> GetServiceAccess(int id, CancellationToken cancellationToken)
    //        => BaseResult(await _mediator.Send(new GetCustomerServiceAccessQuery(id), cancellationToken));

    //    [HttpPut("Customers/{id:int}/ServiceAccess/{serviceKey}")]
    //    [AuthenticationUserAdminByCookie]
    //    public async Task<IActionResult> SetServiceAccess(int id, string serviceKey, [FromQuery] bool isEnabled, CancellationToken cancellationToken)
    //        => BaseResult(await _mediator.Send(
    //            new SetCustomerServiceAccessCommand(id, serviceKey, isEnabled, _baseRequest.RequestInfo.Token), cancellationToken));
    //}

}
