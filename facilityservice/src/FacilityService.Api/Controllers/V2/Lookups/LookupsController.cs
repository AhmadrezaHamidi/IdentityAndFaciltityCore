using FacilityService.Api.Base;
using FacilityService.Api.Attributes;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FacilityService.Api.Controllers.V2.Lookups;

/// <summary>فهرست‌های ثابت پنل؛ به هیچ تبی وابسته نیست.</summary>
[Route("Lookups")]
[ApiVersion("2")]
[SwaggerTag("فهرست‌های ثابت")]
public class LookupsController(ILogger<LookupsController> logger) : FacilityPanelControllerBase(logger)
{
    /// <summary>
    /// عنوان همهٔ enumها. کلیدی لازم ندارد چون چیزی از دادهٔ شرکت‌ها نمی‌گوید — ولی
    /// [RequireFacilityPermission] بدون کلید هست، پس نشست معتبر لازم است و مسیر
    /// از قاعدهٔ «هر مسیر باید تعیین تکلیف شده باشد» بیرون نمی‌افتد.
    /// </summary>
    [HttpGet("Enums")]
    [RequireFacilityPermission]
    [SwaggerOperation(OperationId = "Lookups_Enums", Summary = "عنوان همهٔ enumها",
        Description = "فرانت یک بار می‌خواند و کش می‌کند؛ عنوان وضعیت‌ها را در کد خودش تکرار نمی‌کند.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<EnumCatalogDto>))]
    public async Task<IActionResult> Enums(CancellationToken ct) => HandleResult(await _mediator.Send(new GetEnumCatalogQuery(), ct));
}
