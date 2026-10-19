using FacilityService.Api.Base;
using FacilityService.Api.Attributes;
using FacilityPanel.Api.Attribute;
using FacilityService.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FacilityService.Api.Controllers.V2.Internal;

/// <summary>سرویس‌به‌سرویس برای Identity (Lookups/Companies در Admin.Api). از Gateway منتشر نشود.</summary>
[Route("Internal/Companies")]
[ApiVersion("2")]
[FacilityAnonymous]
[FacilityInternalApiKey]
[SwaggerTag("داخلی")]
public class InternalCompaniesController(ILogger<InternalCompaniesController> logger) : FacilityPanelControllerBase(logger)
{
    [HttpGet("Lookup")]
    [SwaggerOperation(OperationId = "Internal_CompanyLookup", Summary = "Lookup همهٔ شرکت‌ها برای Identity", Description = "هدر X-Api-Key لازم است.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<FacilityLookupItemDto>>))]
    public async Task<IActionResult> Lookup([FromQuery] string? term, CancellationToken ct)
        => HandleResult(await _mediator.Send(new InternalCompanyLookupQuery(term), ct));
}
