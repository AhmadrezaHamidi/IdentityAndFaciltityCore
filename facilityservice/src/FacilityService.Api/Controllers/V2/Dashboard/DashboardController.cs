using FacilityService.Api.Base;
using FacilityService.Api.Attributes;
using FacilityService.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Api.Controllers.V2.Dashboard;

/// <summary>تب «داشبورد مدیریتی».</summary>
[Route("Dashboard")]
[ApiVersion("2")]
[SwaggerTag("داشبورد مدیریتی")]
public class DashboardController(ILogger<DashboardController> logger) : FacilityPanelControllerBase(logger)
{
    [HttpGet]
    [RequireFacilityPermission(K.DashboardView)]
    [SwaggerOperation(OperationId = "Dashboard_Get", Summary = "KPIها و نمودارها",
        Description = "۴ KPI، وضعیت درخواست‌ها، سهم طرح‌ها، اعتبار در برابر تخصیص، تفاهم‌نامه‌ها به تفکیک ماه، عملکرد ماهانه، شرکت‌های حذف‌شده. همه در دامنهٔ کاربر.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<DashboardDto>))]
    public async Task<IActionResult> Get([FromQuery] GetDashboardQuery query, CancellationToken ct) => HandleResult(await _mediator.Send(query, ct));

    [HttpGet("CompanyRanking")]
    [RequireFacilityPermission(K.DashboardView)]
    [SwaggerOperation(OperationId = "Dashboard_Ranking", Summary = "رتبه‌بندی شرکت‌ها", Description = "ماه خالی = تجمعی.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<CompanyRankingDto>>))]
    public async Task<IActionResult> Ranking([FromQuery] GetCompanyRankingQuery query, CancellationToken ct) => HandleResult(await _mediator.Send(query, ct));

    [HttpGet("Months")]
    [RequireFacilityPermission(K.DashboardView)]
    [SwaggerOperation(OperationId = "Dashboard_Months", Summary = "ماه‌های دارای داده (برای فیلتر ماه و ترتیب ستون‌ها)",
        Description = "کلید ماه همیشه قالب 1404/05 دارد؛ همین کلید را به CompanyRanking?month= بدهید. ترتیب: تازه‌ترین اول.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<MonthOptionDto>>))]
    public async Task<IActionResult> Months(CancellationToken ct) => HandleResult(await _mediator.Send(new GetDashboardMonthsQuery(), ct));

    [HttpGet("Exports/{report}")]
    [RequireFacilityPermission(K.DashboardExport)]
    [SwaggerOperation(OperationId = "Dashboard_Export", Summary = "خروجی CSV (۵ دکمه)",
        Description = "Companies، Personnel، LoanRequests، MonthlyAverages، RemovedCompanies — فقط دادهٔ دامنهٔ کاربر.")]
    public async Task<IActionResult> Export(DashboardExport report, CancellationToken ct) => FileResult(await _mediator.Send(new ExportDashboardQuery(report), ct));
}
