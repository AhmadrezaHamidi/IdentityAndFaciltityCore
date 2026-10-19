using FacilityService.Api.Base;
using FacilityService.Api.Attributes;
using FacilityService.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Api.Controllers.V2.Loans;

/// <summary>تب «تخصیص وام».</summary>
[ApiVersion("2")]
[SwaggerTag("تخصیص وام")]
public class LoansController(ILogger<LoansController> logger) : FacilityPanelControllerBase(logger)
{
    [HttpGet("Companies/{companyId:long}/Credit")]
    [RequireFacilityPermission(K.LoansCreditView)]
    [SwaggerOperation(OperationId = "Loans_Credit", Summary = "اعتبار طرح‌ها و بنر وضعیت (US-E4، D2)",
        Description = "بنر: مهلت معرفی نرسیده / امروز روز مجاز انتقال نیست / مجاز است. جدول اعتبار هر طرح تفاهم‌نامه.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<CompanyCreditDto>))]
    public async Task<IActionResult> Credit(long companyId, [FromQuery] DateOnly? baseDate, CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetCompanyCreditQuery(companyId, baseDate), ct));

    [HttpPost("Companies/{companyId:long}/LoanRequests/Preview")]
    [RequireFacilityPermission(K.LoansCreate)]
    [SwaggerOperation(OperationId = "Loans_Preview", Summary = "هینت‌های زندهٔ فرم (US-E5، D6)",
        Description = "مجموع، مجاز بودن، «حداکثر تا N نفر با این مبلغ»، هشدار تجاوز از اعتبار. چیزی ذخیره نمی‌شود.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<LoanRequestPreviewDto>))]
    public async Task<IActionResult> Preview(long companyId, [FromBody] PreviewLoanRequestQuery query, CancellationToken ct)
        => HandleResult(await _mediator.Send(query with { CompanyId = companyId }, ct));

    [HttpPost("Companies/{companyId:long}/LoanRequests")]
    [RequireFacilityPermission(K.LoansCreate)]
    [SwaggerOperation(OperationId = "Loans_Create", Summary = "ثبت درخواست برای پرسنل انتخاب‌شده (US-D3)",
        Description = "مبلغ یکسان یا متفاوت؛ مجموع ≤ مانده اعتبار طرح؛ تاریخ مبنا در یک ماه گذشته بر اساس روز افتتاح حساب. " +
                      "درخواست‌ها به صف «تأیید مدیر شرکت» می‌روند.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<LoanBatchDto>))]
    [SwaggerResponse(400, "قاعدهٔ کسب‌وکار رد شد: مجموع مبالغ بیشتر از مانده اعتبار طرح، تاریخ مبنا خارج از بازهٔ مجاز (baseDateMin تا baseDateMax در سرویس اعتبار)، امروز روز مجاز انتقال اعتبار نیست، قرارداد فعال نیست، مهلت معرفی پرسنل نرسیده، پرسنل تأییدنشده یا تکراری. پیام فارسی در message است.",
        typeof(FacilityBaseMessageResponse<object>))]
    [SwaggerResponse(404, "شرکت یا پرسنل در دامنهٔ شما یافت نشد.", typeof(FacilityBaseMessageResponse<object>))]
    public async Task<IActionResult> Create(long companyId, [FromBody] CreateLoanRequestsCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { CompanyId = companyId }, ct));

    [HttpGet("LoanRequests")]
    [RequireFacilityPermission(K.LoansView)]
    [SwaggerOperation(OperationId = "Loans_Search", Summary = "درخواست‌های وام",
        Description = "همهٔ درخواست‌های دامنهٔ کاربر؛ برای «درخواست‌های وام این شرکت» CompanyId بده. با تاریخ ثبت، تاریخ تأیید/رد و وضعیت.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<FacilityPagedResponse<LoanRequestListItemDto>>))]
    public async Task<IActionResult> Search([FromQuery] SearchLoanRequestsQuery query, CancellationToken ct) => HandleResult(await _mediator.Send(query, ct));

    [HttpGet("LoanRequests/{id:long}")]
    [RequireFacilityPermission(K.LoansView, K.ManagerApprovalView, K.CreditApprovalView)]
    [SwaggerOperation(OperationId = "Loans_Get", Summary = "جزئیات درخواست و تاریخچهٔ تصمیم‌ها")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<LoanRequestDetailsDto>))]
    public async Task<IActionResult> Get(long id, CancellationToken ct) => HandleResult(await _mediator.Send(new GetLoanRequestQuery(id), ct));

    [HttpPost("LoanBatches/{batchId:long}/Members")]
    [RequireFacilityPermission(K.LoansMembersEdit)]
    [SwaggerOperation(OperationId = "Loans_AddMember", Summary = "افزودن فرد به درخواست (US-D4)", Description = "قبل از تأیید اعتبارات.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<LoanBatchDto>))]
    public async Task<IActionResult> AddMember(long batchId, [FromBody] AddLoanBatchMemberCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { BatchId = batchId }, ct));

    [HttpDelete("LoanBatches/{batchId:long}/Members/{requestId:long}")]
    [RequireFacilityPermission(K.LoansMembersEdit)]
    [SwaggerOperation(OperationId = "Loans_RemoveMember", Summary = "حذف فرد از درخواست (US-D4)", Description = "قبل از هر تصمیم.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<LoanBatchDto>))]
    public async Task<IActionResult> RemoveMember(long batchId, long requestId, CancellationToken ct)
        => HandleResult(await _mediator.Send(new RemoveLoanBatchMemberCommand(batchId, requestId), ct));

    [HttpGet("LoanRequests/{id:long}/Documents/{fileId:long}")]
    [RequireFacilityPermission(K.LoansView)]
    [SwaggerOperation(OperationId = "Loans_DocumentDownload", Summary = "دانلود مستند درخواست",
        Description = "fileId همان id عضوهای documents در ردیف فهرست است.")]
    public async Task<IActionResult> DownloadDocument(long id, long fileId, CancellationToken ct)
        => FileResult(await _mediator.Send(new DownloadLoanDocumentQuery(id, fileId), ct));

    [HttpPost("LoanRequests/{id:long}/Cancel")]
    [RequireFacilityPermission(K.LoansCancel)]
    [SwaggerOperation(OperationId = "Loans_Cancel", Summary = "لغو درخواست یک نفر (US-D5)", Description = "فقط قبل از تأیید اعتبارات؛ دلیل از فهرست + توضیح.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<LoanRequestListItemDto>))]
    public async Task<IActionResult> Cancel(long id, [FromBody] CancelLoanRequestCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { Id = id }, ct));
}
