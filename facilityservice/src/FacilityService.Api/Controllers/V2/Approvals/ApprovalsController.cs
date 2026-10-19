using FacilityService.Api.Base;
using FacilityService.Api.Attributes;
using FacilityService.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Api.Controllers.V2.Approvals;

/// <summary>تب‌های «تأیید مدیر شرکت» و «تأیید اعتبارات» و مرحلهٔ «تأیید شعبهٔ دیجیتال».</summary>
[ApiVersion("2")]
[SwaggerTag("تأییدها")]
public class ApprovalsController(ILogger<ApprovalsController> logger) : FacilityPanelControllerBase(logger)
{
    // ── تأیید مدیر شرکت ──

    [HttpGet("ManagerApprovals")]
    [RequireFacilityPermission(K.ManagerApprovalView)]
    [SwaggerOperation(OperationId = "Approvals_ManagerQueue", Summary = "صف تأیید مدیر شرکت",
        Description = "فقط درخواست‌هایی که اینجا تأیید شوند به «تأیید اعتبارات» می‌روند.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<FacilityPagedResponse<LoanRequestListItemDto>>))]
    public async Task<IActionResult> ManagerQueue([FromQuery] GetApprovalQueueQuery query, CancellationToken ct)
        => HandleResult(await _mediator.Send(query with { Stage = ApprovalStage.Manager }, ct));

    [HttpPost("ManagerApprovals/Decisions")]
    [RequireFacilityPermission(K.ManagerApprovalApprove, K.ManagerApprovalReject)]
    [SwaggerOperation(OperationId = "Approvals_ManagerDecide", Summary = "تأیید/رد گروهی یا تکی مدیر شرکت",
        Description = "تأیید به manager-approval.approve و رد به manager-approval.reject نیاز دارد. رد: دلیل و توضیح الزامی (از «دلایل رد درخواست وام»، همان فهرست اعتبارات). "
                    + "اجرای ناقص مجاز است: ردیف‌های انجام‌نشده در skipped با دلیل برمی‌گردند.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<DecisionResultDto>))]
    public async Task<IActionResult> ManagerDecide([FromBody] DecideLoanRequestsCommand command, CancellationToken ct)
        => RequireAnywhere(command.Decision == DecisionType.Approve ? K.ManagerApprovalApprove : K.ManagerApprovalReject)
           ?? HandleResult(await _mediator.Send(command with { Stage = ApprovalStage.Manager }, ct));

    // ── تأیید اعتبارات ──

    [HttpGet("CreditApprovals")]
    [RequireFacilityPermission(K.CreditApprovalView)]
    [SwaggerOperation(OperationId = "Approvals_CreditQueue", Summary = "صف تأیید اعتبارات (US-B7)", Description = "کاربر شعبه فقط شرکت‌های شعبهٔ خود؛ فیلتر شرکت.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<FacilityPagedResponse<LoanRequestListItemDto>>))]
    public async Task<IActionResult> CreditQueue([FromQuery] GetApprovalQueueQuery query, CancellationToken ct)
        => HandleResult(await _mediator.Send(query with { Stage = ApprovalStage.Credit }, ct));

    [HttpPost("CreditApprovals/Decisions")]
    [Consumes("multipart/form-data")]
    [RequireFacilityPermission(K.CreditApprovalApprove, K.CreditApprovalReject)]
    [SwaggerOperation(OperationId = "Approvals_CreditDecide", Summary = "تأیید/رد اعتبارات، تکی یا گروهی (US-B8)",
        Description = "یک تراکنش، ولی اجرای ناقص مجاز است: ردیف‌هایی که در این مرحله نیستند یا خارج از دامنهٔ شمایند در skipped با دلیل برمی‌گردند و processed تعداد انجام‌شده است. "
                    + "رد: دلیل از فهرست و توضیح الزامی و برای همه یکسان. پیوست مصوبه اختیاری. "
                    + "postStage را در همین تصمیم بفرستید تا مرحلهٔ پس از تأیید گروهی ثبت شود (کلید facility.credit-approval.stage لازم است؛ برای عدم اعطای غیرحضوری postStageReasonId الزامی).")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<DecisionResultDto>))]
    public async Task<IActionResult> CreditDecide([FromForm] DecideLoanRequestsCommand command, IFormFile? attachment, CancellationToken ct)
        => RequireAnywhere(command.Decision == DecisionType.Approve ? K.CreditApprovalApprove : K.CreditApprovalReject)
           ?? HandleResult(await _mediator.Send(command with { Stage = ApprovalStage.Credit, Attachment = await attachment.ToUploadAsync(ct) }, ct));

    [HttpPut("CreditApprovals/{requestId:long}/PostApprovalStage")]
    [RequireFacilityPermission(K.CreditApprovalStage)]
    [SwaggerOperation(OperationId = "Approvals_PostStage", Summary = "تغییر مرحلهٔ پس از تأیید (برای درخواست تأییدشده)",
        Description = "تشکیل پرونده در شعبه / دریافت تسهیلات از امید بانک / دریافت در شعبه / عدم اعطای غیرحضوری (دلیل الزامی). "
                    + "برای ثبت مرحله هم‌زمان با تأیید، postStage را به سرویس تصمیم بدهید؛ این سرویس برای تغییر بعدی است.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<LoanRequestListItemDto>))]
    public async Task<IActionResult> PostStage(long requestId, [FromBody] SetPostApprovalStageCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { RequestId = requestId }, ct));

    // ── تأیید شعبهٔ دیجیتال (اگر در اطلاعات پایه فعال باشد) ──

    [HttpGet("DigitalApprovals")]
    [RequireFacilityPermission(K.DigitalApprovalDecide, K.CreditApprovalView)]
    [SwaggerOperation(OperationId = "Approvals_DigitalQueue", Summary = "صف تأیید شعبهٔ دیجیتال")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<FacilityPagedResponse<LoanRequestListItemDto>>))]
    public async Task<IActionResult> DigitalQueue([FromQuery] GetApprovalQueueQuery query, CancellationToken ct)
        => HandleResult(await _mediator.Send(query with { Stage = ApprovalStage.Digital }, ct));

    [HttpPost("DigitalApprovals/Decisions")]
    [Consumes("multipart/form-data")]
    [RequireFacilityPermission(K.DigitalApprovalDecide)]
    [SwaggerOperation(OperationId = "Approvals_DigitalDecide", Summary = "تأیید/رد شعبهٔ دیجیتال")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<DecisionResultDto>))]
    public async Task<IActionResult> DigitalDecide([FromForm] DecideLoanRequestsCommand command, IFormFile? attachment, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { Stage = ApprovalStage.Digital, Attachment = await attachment.ToUploadAsync(ct) }, ct));

    // ── شمارهٔ روی تب‌ها ──

    [HttpGet("Approvals/PendingCounts")]
    [RequireFacilityPermission]
    [SwaggerOperation(OperationId = "Approvals_PendingCounts", Summary = "تعداد در انتظار روی تب‌ها",
        Description = "برای مراحلی که کاربر مجوز دیدنشان را ندارد null برمی‌گردد.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<PendingCountsDto>))]
    public async Task<IActionResult> PendingCounts(CancellationToken ct) => HandleResult(await _mediator.Send(new GetPendingCountsQuery(), ct));
}
