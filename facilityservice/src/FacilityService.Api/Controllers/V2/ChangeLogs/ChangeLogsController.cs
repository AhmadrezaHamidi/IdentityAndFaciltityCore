using FacilityService.Api.Base;
using FacilityService.Api.Attributes;
using FacilityService.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Api.Controllers.V2.ChangeLogs;

/// <summary>تب «گزارش تغییرات». دستهٔ «کاربران» از Identity: GET UserManagement/SecurityAudit.</summary>
[Route("ChangeLogs")]
[ApiVersion("2")]
[SwaggerTag("گزارش تغییرات")]
public class ChangeLogsController(ILogger<ChangeLogsController> logger) : FacilityPanelControllerBase(logger)
{
    [HttpGet]
    [RequireFacilityPermission(K.AuditView)]
    [SwaggerOperation(OperationId = "ChangeLogs_Search", Summary = "لاگ عملیات", Description = "فیلتر نوع (شرکت/پرسنل/وام)، شرکت، کاربر، تاریخ؛ در دامنهٔ کاربر.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<FacilityPagedResponse<ChangeLogItemDto>>))]
    public async Task<IActionResult> Search([FromQuery] SearchChangeLogsQuery query, CancellationToken ct) => HandleResult(await _mediator.Send(query, ct));

    [HttpGet("Users")]
    [RequireFacilityPermission(K.AuditView)]
    [SwaggerOperation(OperationId = "ChangeLogs_Users", Summary = "کاربران دارای ردیف در گزارش (برای سلکت فیلتر)",
        Description = "فقط کاربرانی که ردیف‌هایشان در دامنهٔ کاربر جاری دیده می‌شود؛ panelUserId همان چیزی است که به فیلتر داده می‌شود.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<RegistrarDto>>))]
    public async Task<IActionResult> Users(CancellationToken ct) => HandleResult(await _mediator.Send(new GetChangeLogUsersQuery(), ct));

    [HttpGet("Export")]
    [RequireFacilityPermission(K.AuditExport)]
    [SwaggerOperation(OperationId = "ChangeLogs_Export", Summary = "خروجی گزارش (CSV با UTF-8 BOM، قابل باز شدن در Excel)",
        Description = "فیلترها دقیقاً همان فیلترهای لیست‌اند (شامل جستجوی متنی و فیلتر کاربر). ⚠ اگر xlsx لازم است باید تصمیم گرفته شود.")]
    [SwaggerResponse(200, "فایل CSV")]
    public async Task<IActionResult> Export([FromQuery] ExportChangeLogsQuery query, CancellationToken ct) => FileResult(await _mediator.Send(query, ct));

    [HttpPost("{id:long}/Restore")]
    [RequireFacilityPermission(K.AuditRestore)]
    [SwaggerOperation(OperationId = "ChangeLogs_Restore", Summary = "بازگردانی عملیات (US-A4)", Description = "فقط ادمین. ⚠ Q16: عملیات قابل بازگردانی تعیین شود.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<ChangeLogItemDto>))]
    public async Task<IActionResult> Restore(long id, [FromBody] RestoreChangeCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { Id = id }, ct));
}
