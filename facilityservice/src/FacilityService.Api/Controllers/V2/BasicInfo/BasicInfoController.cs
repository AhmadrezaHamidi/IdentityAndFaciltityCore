using FacilityService.Api.Base;
using FacilityService.Api.Attributes;
using FacilityService.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Api.Controllers.V2.BasicInfo;

/// <summary>تب «اطلاعات پایه طرح».</summary>
[Route("BasicInfo")]
[ApiVersion("2")]
[SwaggerTag("اطلاعات پایه طرح")]
public class BasicInfoController(ILogger<BasicInfoController> logger) : FacilityPanelControllerBase(logger)
{
    [HttpGet("Schemes")]
    [RequireFacilityPermission(K.BasicInfoView, K.CompaniesView, K.LoansCreditView)]
    [SwaggerOperation(OperationId = "BasicInfo_Schemes", Summary = "طرح‌های کلان (نگین میثاق، شجره طیبه، ...)")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<SchemeDto>>))]
    public async Task<IActionResult> Schemes(CancellationToken ct) => HandleResult(await _mediator.Send(new GetSchemesQuery(), ct));

    [HttpGet("{schemeId:int}")]
    [RequireFacilityPermission(K.BasicInfoView, K.CompaniesCreate, K.CompaniesEdit, K.LoansCreditView)]
    [SwaggerOperation(OperationId = "BasicInfo_Get", Summary = "تنظیمات یک طرح (نرخ‌ها، مدت‌ها، ضرایب، جدول درصد قابل استفاده)")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<BasicInfoDto>))]
    public async Task<IActionResult> Get(int schemeId, CancellationToken ct) => HandleResult(await _mediator.Send(new GetBasicInfoQuery(schemeId), ct));

    [HttpPut("{schemeId:int}")]
    [RequireFacilityPermission(K.BasicInfoEdit)]
    [SwaggerOperation(OperationId = "BasicInfo_Update", Summary = "ذخیرهٔ تنظیمات پایه", Description = "فقط ادمین (پیش‌فرض). همهٔ محاسبات اعتبار از همین‌جا خوانده می‌شود.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<BasicInfoDto>))]
    public async Task<IActionResult> Update(int schemeId, [FromBody] UpdateBasicInfoCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { SchemeId = schemeId }, ct));

    [HttpGet("{schemeId:int}/Plans")]
    [RequireFacilityPermission(K.BasicInfoView, K.CompaniesView, K.LoansCreditView)]
    [SwaggerOperation(OperationId = "BasicInfo_Plans", Summary = "طرح‌های قابل انتخاب (عقد × نرخ × مدت) + سقف تعداد طرح هر شرکت",
        Description = "برای فرم ثبت شرکت و انتخاب طرح در تخصیص وام. maxPlansPerCompany همان سقفی است که فرانت برای محدود کردن انتخاب لازم دارد.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<PlanListDto>))]
    public async Task<IActionResult> Plans(int schemeId, CancellationToken ct) => HandleResult(await _mediator.Send(new GetPlansQuery(schemeId), ct));

    [HttpGet("Reasons")]
    [RequireFacilityPermission(K.BasicInfoView, K.CompaniesContractCancel, K.CompaniesDocumentsUpload, K.LoansCancel,
        K.ManagerApprovalReject, K.CreditApprovalReject, K.CreditApprovalStage, K.PersonnelApprove)]
    [SwaggerOperation(OperationId = "BasicInfo_Reasons", Summary = "فهرست‌های داینامیک",
        Description = "دلایل لغو قرارداد، رد وام، عدم اعطای غیرحضوری، لغو درخواست، رد پرسنل، نوع مستند. برای Dropdown مدال‌ها.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<ReasonDto>>))]
    public async Task<IActionResult> Reasons([FromQuery] ReasonType type, [FromQuery] bool includeInactive, CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetReasonsQuery(type, includeInactive), ct));

    [HttpPost("Reasons")]
    [RequireFacilityPermission(K.BasicInfoEdit)]
    [SwaggerOperation(OperationId = "BasicInfo_ReasonCreate", Summary = "افزودن دلیل جدید")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<ReasonDto>))]
    public async Task<IActionResult> CreateReason([FromBody] CreateReasonCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command, ct));

    [HttpPut("Reasons/{id:int}")]
    [RequireFacilityPermission(K.BasicInfoEdit)]
    [SwaggerOperation(OperationId = "BasicInfo_ReasonUpdate", Summary = "ویرایش/غیرفعال کردن دلیل")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<ReasonDto>))]
    public async Task<IActionResult> UpdateReason(int id, [FromBody] UpdateReasonCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { Id = id }, ct));

    [HttpDelete("Reasons/{id:int}")]
    [RequireFacilityPermission(K.BasicInfoEdit)]
    [SwaggerOperation(OperationId = "BasicInfo_ReasonDelete", Summary = "حذف دلیل (نرم)")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<bool>))]
    public async Task<IActionResult> DeleteReason(int id, CancellationToken ct) => HandleResult(await _mediator.Send(new DeleteReasonCommand(id), ct));
}
