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
    [SwaggerOperation(OperationId = "BasicInfo_Schemes", Summary = "کشویی طرح‌های کلان (نگین میثاق، شجره طیبه، ...)",
        Description = "فقط شناسه و عنوان، برای کشویی فرم شرکت. برای جدول «اطلاعات پایه طرح» از Schemes/Search استفاده کنید.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<SchemeDto>>))]
    public async Task<IActionResult> Schemes(CancellationToken ct) => HandleResult(await _mediator.Send(new GetSchemesQuery(), ct));

    // ── فرم طرح: یک سرویس برای خواندن، یک سرویس برای نوشتن ──

    [HttpGet("Schemes/Search")]
    [RequireFacilityPermission(K.BasicInfoView)]
    [SwaggerOperation(OperationId = "BasicInfo_SchemeSearch", Summary = "جدول طرح‌ها (صفحه‌بندی‌شده)",
        Description = "همان ستون‌هایی که در جدول دیده می‌شود: نام، نرخ قرض‌الحسنه، نرخ‌های مرابحه، مدت‌های بازپرداخت، مدت قرارداد، حداکثر طرح، وضعیت و تعداد شرکت. includeInactive فقط برای کسی که اطلاعات پایه را ویرایش می‌کند کار می‌کند.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<FacilityPagedResponse<SchemeListItemDto>>))]
    public async Task<IActionResult> SchemeSearch([FromQuery] SearchSchemesQuery query, CancellationToken ct)
        => HandleResult(await _mediator.Send(query, ct));

    [HttpGet("Schemes/{schemeId:int}")]
    [RequireFacilityPermission(K.BasicInfoView)]
    [SwaggerOperation(OperationId = "BasicInfo_SchemeGet", Summary = "جزئیات طرح — همهٔ ۱۴ مورد فرم",
        Description = "هویت طرح، تنظیمات (نرخ‌ها، مدت‌ها، ضرایب، جدول درصد قابل استفاده) و فهرست‌های دلایل همین طرح، در یک پاسخ.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<SchemeFormDto>))]
    public async Task<IActionResult> SchemeGet(int schemeId, CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetSchemeQuery(schemeId), ct));

    [HttpPost("Schemes")]
    [RequireFacilityPermission(K.BasicInfoEdit)]
    [SwaggerOperation(OperationId = "BasicInfo_SchemeCreate", Summary = "ایجاد طرح — همهٔ موارد فرم در یک درخواست",
        Description = "code الزامی است و بعداً عوض نمی‌شود. فهرست‌های دلایل به‌صورت آرایهٔ عنوان فرستاده می‌شوند و به همین طرح تعلق می‌گیرند.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<SchemeFormDto>))]
    [SwaggerResponse(400, "کد تکراری، کد خالی، یا نقض یکی از بازه‌های فرم (نرخ، مدت، ضریب، درصد قابل استفاده).", typeof(FacilityBaseMessageResponse<object>))]
    public async Task<IActionResult> SchemeCreate([FromBody] SaveSchemeCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { SchemeId = 0 }, ct));

    [HttpPut("Schemes/{schemeId:int}")]
    [RequireFacilityPermission(K.BasicInfoEdit)]
    [SwaggerOperation(OperationId = "BasicInfo_SchemeUpdate", Summary = "ویرایش طرح — همهٔ موارد فرم در یک درخواست",
        Description = "rowVersion الزامی است (قفل خوش‌بینانه). code نادیده گرفته می‌شود. فهرستی که null بیاید تغییر نمی‌کند؛ فهرستی که بیاید جایگزین می‌شود و موارد برداشته‌شده غیرفعال (نه پاک) می‌شوند.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<SchemeFormDto>))]
    [SwaggerResponse(409, "طرح در این فاصله عوض شده؛ دوباره بخوانید.", typeof(FacilityBaseMessageResponse<object>))]
    public async Task<IActionResult> SchemeUpdate(int schemeId, [FromBody] SaveSchemeCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { SchemeId = schemeId }, ct));

    [HttpDelete("Schemes/{schemeId:int}")]
    [RequireFacilityPermission(K.BasicInfoEdit)]
    [SwaggerOperation(OperationId = "BasicInfo_SchemeDelete", Summary = "حذف طرح (نرم)",
        Description = "آخرین طرح فعال و طرحی که شرکت فعال دارد حذف نمی‌شوند.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<bool>))]
    [SwaggerResponse(400, "آخرین طرح فعال است، یا شرکت فعال دارد.", typeof(FacilityBaseMessageResponse<object>))]
    public async Task<IActionResult> SchemeDelete(int schemeId, CancellationToken ct)
        => HandleResult(await _mediator.Send(new DeleteSchemeCommand(schemeId), ct));

    [HttpGet("{schemeId:int}")]
    [RequireFacilityPermission(K.BasicInfoView, K.CompaniesCreate, K.CompaniesEdit, K.LoansCreditView)]
    [SwaggerOperation(OperationId = "BasicInfo_Get", Summary = "فقط تنظیمات یک طرح (نرخ‌ها، مدت‌ها، ضرایب، جدول درصد قابل استفاده)",
        Description = "برای فرم طرح از Schemes/{schemeId} استفاده کنید؛ آن یکی هویت طرح و فهرست‌های دلایلش را هم می‌دهد.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<BasicInfoDto>))]
    public async Task<IActionResult> Get(int schemeId, CancellationToken ct) => HandleResult(await _mediator.Send(new GetBasicInfoQuery(schemeId), ct));

    [HttpPut("{schemeId:int}")]
    [RequireFacilityPermission(K.BasicInfoEdit)]
    [SwaggerOperation(OperationId = "BasicInfo_Update", Summary = "ذخیرهٔ فقط تنظیمات پایه",
        Description = "فقط ادمین (پیش‌فرض). همهٔ محاسبات اعتبار از همین‌جا خوانده می‌شود. برای ذخیرهٔ کل فرم طرح (با نام و فهرست‌های دلایل) از PUT Schemes/{schemeId} استفاده کنید.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<BasicInfoDto>))]
    public async Task<IActionResult> Update(int schemeId, [FromBody] UpdateBasicInfoCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { SchemeId = schemeId }, ct));

    [HttpGet("{schemeId:int}/Plans")]
    [RequireFacilityPermission(K.BasicInfoView, K.CompaniesView, K.LoansCreditView)]
    [SwaggerOperation(OperationId = "BasicInfo_Plans", Summary = "طرح‌های قابل انتخاب (عقد × نرخ × مدت) + سقف تعداد طرح هر شرکت",
        Description = "برای فرم ثبت شرکت و انتخاب طرح در تخصیص وام. maxPlansPerCompany همان سقفی است که فرانت برای محدود کردن انتخاب لازم دارد.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<PlanListDto>))]
    public async Task<IActionResult> Plans(int schemeId, CancellationToken ct) => HandleResult(await _mediator.Send(new GetPlansQuery(schemeId), ct));

    // ── فهرست‌های داینامیک، در دامنهٔ طرح ──
    // هر دلیل به یک طرح تعلق دارد (سند فرانت، موارد ۱۱ تا ۱۴ فرم طرح). برای ویرایش دسته‌ای
    // کل فهرست، PUT Schemes/{id} را با آرایهٔ عنوان‌ها بزنید؛ این مسیرها برای تغییر یک مورد است.

    [HttpGet("Schemes/{schemeId:int}/Reasons")]
    [RequireFacilityPermission(K.BasicInfoView, K.CompaniesContractCancel, K.CompaniesDocumentsUpload, K.LoansCancel,
        K.ManagerApprovalReject, K.CreditApprovalReject, K.CreditApprovalStage, K.PersonnelApprove)]
    [SwaggerOperation(OperationId = "BasicInfo_SchemeReasons", Summary = "یک فهرست داینامیک از یک طرح",
        Description = "دلایل لغو قرارداد، رد وام، عدم اعطای غیرحضوری، لغو درخواست، رد پرسنل، نوع مستند. برای Dropdown مدال‌ها — طرح را از شرکت بگیرید.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<ReasonDto>>))]
    public async Task<IActionResult> SchemeReasons(int schemeId, [FromQuery] ReasonType type, [FromQuery] bool includeInactive, CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetReasonsQuery(schemeId, type, includeInactive), ct));

    [HttpGet("Reasons")]
    [RequireFacilityPermission(K.BasicInfoView, K.CompaniesContractCancel, K.CompaniesDocumentsUpload, K.LoansCancel,
        K.ManagerApprovalReject, K.CreditApprovalReject, K.CreditApprovalStage, K.PersonnelApprove)]
    [SwaggerOperation(OperationId = "BasicInfo_Reasons", Summary = "فهرست داینامیک طرح پیش‌فرض (برای سازگاری)",
        Description = "⚠ فهرست‌ها دیگر سراسری نیستند. این مسیر تا وقتی فقط یک طرح فعال باشد کار می‌کند و با طرح دوم خطای روشن می‌دهد. مسیر درست: Schemes/{schemeId}/Reasons.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<ReasonDto>>))]
    [SwaggerResponse(400, "بیش از یک طرح فعال وجود دارد؛ طرح را مشخص کنید.", typeof(FacilityBaseMessageResponse<object>))]
    public async Task<IActionResult> Reasons([FromQuery] ReasonType type, [FromQuery] bool includeInactive, CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetReasonsQuery(0, type, includeInactive), ct));

    [HttpPost("Schemes/{schemeId:int}/Reasons")]
    [RequireFacilityPermission(K.BasicInfoEdit)]
    [SwaggerOperation(OperationId = "BasicInfo_ReasonCreate", Summary = "افزودن یک مورد به فهرست طرح")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<ReasonDto>))]
    public async Task<IActionResult> CreateReason(int schemeId, [FromBody] CreateReasonCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { SchemeId = schemeId }, ct));

    [HttpPut("Schemes/{schemeId:int}/Reasons/{id:int}")]
    [RequireFacilityPermission(K.BasicInfoEdit)]
    [SwaggerOperation(OperationId = "BasicInfo_ReasonUpdate", Summary = "ویرایش/غیرفعال کردن یک مورد")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<ReasonDto>))]
    public async Task<IActionResult> UpdateReason(int schemeId, int id, [FromBody] UpdateReasonCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { SchemeId = schemeId, Id = id }, ct));

    [HttpDelete("Schemes/{schemeId:int}/Reasons/{id:int}")]
    [RequireFacilityPermission(K.BasicInfoEdit)]
    [SwaggerOperation(OperationId = "BasicInfo_ReasonDelete", Summary = "حذف یک مورد (نرم)")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<bool>))]
    public async Task<IActionResult> DeleteReason(int schemeId, int id, CancellationToken ct)
        => HandleResult(await _mediator.Send(new DeleteReasonCommand(schemeId, id), ct));
}
