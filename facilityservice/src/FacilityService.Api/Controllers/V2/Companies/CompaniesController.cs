using FacilityService.Api.Base;
using FacilityService.Api.Attributes;
using FacilityService.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Api.Controllers.V2.Companies;

/// <summary>تب «مدیریت شرکت‌ها» + میانگین موجودی و مستندات.</summary>
[Route("Companies")]
[ApiVersion("2")]
[SwaggerTag("مدیریت شرکت‌ها")]
public class CompaniesController(ILogger<CompaniesController> logger) : FacilityPanelControllerBase(logger)
{
    [HttpGet]
    [RequireFacilityPermission(K.CompaniesView)]
    [SwaggerOperation(OperationId = "Companies_Search", Summary = "فهرست شرکت‌ها", Description = "فقط شرکت‌های دامنهٔ کاربر (شعبهٔ خود/همه). جستجو در نام، شناسهٔ ملی، شمارهٔ تفاهم‌نامه.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<CompanySearchResultDto>))]
    public async Task<IActionResult> Search([FromQuery] SearchCompaniesQuery query, CancellationToken ct) => HandleResult(await _mediator.Send(query, ct));

    [HttpGet("Lookup")]
    [RequireFacilityPermission(K.CompaniesView, K.PersonnelView, K.LoansView, K.ManagerApprovalView, K.CreditApprovalView, K.DashboardView, K.AuditView)]
    [SwaggerOperation(OperationId = "Companies_Lookup", Summary = "Dropdown «انتخاب شرکت»", Description = "برای نقش شرکت فقط شرکت خودش (قفل).")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<FacilityLookupItemDto>>))]
    public async Task<IActionResult> Lookup([FromQuery] string? term, [FromQuery] bool activeOnly = true, CancellationToken ct = default)
        => HandleResult(await _mediator.Send(new GetCompanyLookupQuery(term, activeOnly), ct));

    [HttpGet("{id:long}")]
    [RequireFacilityPermission(K.CompaniesView)]
    [SwaggerOperation(OperationId = "Companies_Get", Summary = "جزئیات شرکت و ثبت‌کننده (US-B6)")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<CompanyDetailsDto>))]
    public async Task<IActionResult> Get(long id, CancellationToken ct) => HandleResult(await _mediator.Send(new GetCompanyQuery(id), ct));

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequireFacilityPermission(K.CompaniesCreate)]
    [SwaggerOperation(OperationId = "Companies_Create", Summary = "ثبت شرکت جدید (US-B2)", Description = "فرم + فایل تفاهم‌نامه. شعبهٔ شرکت = شعبهٔ کاربر ثبت‌کننده.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<CompanyDetailsDto>))]
    public async Task<IActionResult> Create([FromForm] CreateCompanyCommand command, IFormFile? agreementFile, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { AgreementFile = await agreementFile.ToUploadAsync(ct) }, ct));

    [HttpPut("{id:long}")]
    [Consumes("multipart/form-data")]
    [RequireFacilityPermission(K.CompaniesEdit)]
    [SwaggerOperation(OperationId = "Companies_Update", Summary = "ویرایش شرکت")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<CompanyDetailsDto>))]
    public async Task<IActionResult> Update(long id, [FromForm] UpdateCompanyCommand command, IFormFile? agreementFile, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { Id = id, AgreementFile = await agreementFile.ToUploadAsync(ct) }, ct));

    [HttpDelete("{id:long}")]
    [RequireFacilityPermission(K.CompaniesDelete)]
    [SwaggerOperation(OperationId = "Companies_Delete", Summary = "حذف شرکت (نرم)", Description = "در داشبورد «شرکت‌های حذف‌شده» می‌ماند.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<bool>))]
    public async Task<IActionResult> Delete(long id, [FromBody] DeleteCompanyCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { Id = id }, ct));

    [HttpPost("{id:long}/Cancel")]
    [Consumes("multipart/form-data")]
    [RequireFacilityPermission(K.CompaniesContractCancel)]
    [SwaggerOperation(OperationId = "Companies_CancelContract", Summary = "لغو قرارداد (مدال تصمیم)", Description = "تاریخ، دلیل از فهرست، توضیح و فایل نامه الزامی.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<CompanyDetailsDto>))]
    public async Task<IActionResult> Cancel(long id, [FromForm] CancelCompanyContractCommand command, IFormFile? letterFile, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { Id = id, LetterFile = await letterFile.ToUploadAsync(ct) }, ct));

    [HttpGet("{id:long}/AgreementFile")]
    [RequireFacilityPermission(K.CompaniesView)]
    [SwaggerOperation(OperationId = "Companies_AgreementFile", Summary = "دانلود فایل تفاهم‌نامه")]
    public async Task<IActionResult> AgreementFile(long id, CancellationToken ct) => FileResult(await _mediator.Send(new DownloadAgreementFileQuery(id), ct));

    // ── میانگین موجودی و مستندات ──

    [HttpGet("{id:long}/Balances")]
    [RequireFacilityPermission(K.CompaniesView)]
    [SwaggerOperation(OperationId = "Companies_Balances", Summary = "میانگین موجودی ماه‌های سال جاری قرارداد + مانده اعتبار")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<CompanyBalancesDto>))]
    public async Task<IActionResult> Balances(long id, CancellationToken ct) => HandleResult(await _mediator.Send(new GetCompanyBalancesQuery(id), ct));

    [HttpPut("{id:long}/Balances")]
    [Consumes("multipart/form-data")]
    [RequireFacilityPermission(K.CompaniesBalanceEdit)]
    [SwaggerOperation(OperationId = "Companies_BalancesSave", Summary = "ذخیرهٔ میانگین‌ها همراه مستندش (US-B4، US-B5)",
        Description = "فقط کاربر شعبه (شعبهٔ خود) و ادمین. ⚠ بدنه multipart شد: اگر مبلغی عوض شود، documentTypeId و حداقل یک فایل الزامی است و هر دو در یک تراکنش ثبت می‌شوند. "
                    + "یک مستند همهٔ ماه‌های تغییریافتهٔ همین ذخیره را پوشش می‌دهد. اگر هیچ مبلغی عوض نشود، مستند لازم نیست. مبلغ قبلی و جدید در گزارش تغییرات ثبت می‌شود.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<CompanyBalancesDto>))]
    [SwaggerResponse(400, "مستند نیامده، نوع مستند نامعتبر، ماه خارج از سال جاری قرارداد، یا مبلغ نامعتبر.", typeof(FacilityBaseMessageResponse<object>))]
    public async Task<IActionResult> SaveBalances(long id, [FromForm] UpdateCompanyBalancesCommand command, List<IFormFile> files, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { CompanyId = id, Files = await files.ToUploadsAsync(ct) }, ct));

    [HttpGet("{id:long}/Balances/Documents")]
    [RequireFacilityPermission(K.CompaniesDocumentsView)]
    [SwaggerOperation(OperationId = "Companies_BalanceDocuments", Summary = "مستندات میانگین (US-B5)")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<BalanceDocumentDto>>))]
    public async Task<IActionResult> Documents(long id, [FromQuery] string? month, CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetBalanceDocumentsQuery(id, month), ct));

    [HttpPost("{id:long}/Balances/Documents")]
    [Consumes("multipart/form-data")]
    [RequireFacilityPermission(K.CompaniesDocumentsUpload)]
    [SwaggerOperation(OperationId = "Companies_BalanceDocumentsUpload", Summary = "الصاق یک یا چند مستند", Description = "pdf, doc, docx, xls, xlsx, zip, jpg, png؛ هر فایل تا ۵ مگابایت.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<IReadOnlyList<BalanceDocumentDto>>))]
    public async Task<IActionResult> UploadDocuments(long id, [FromForm] UploadBalanceDocumentsCommand command, List<IFormFile> files, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { CompanyId = id, Files = await files.ToUploadsAsync(ct) }, ct));

    [HttpGet("{id:long}/Balances/Documents/{documentId:long}")]
    [RequireFacilityPermission(K.CompaniesDocumentsView)]
    [SwaggerOperation(OperationId = "Companies_BalanceDocumentDownload", Summary = "دانلود مستند")]
    public async Task<IActionResult> DownloadDocument(long id, long documentId, CancellationToken ct)
        => FileResult(await _mediator.Send(new DownloadBalanceDocumentQuery(id, documentId), ct));

    [HttpDelete("{id:long}/Balances/Documents/{documentId:long}")]
    [RequireFacilityPermission(K.CompaniesDocumentsDelete)]
    [SwaggerOperation(OperationId = "Companies_BalanceDocumentDelete", Summary = "حذف مستند (فقط ادمین)")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<bool>))]
    public async Task<IActionResult> DeleteDocument(long id, long documentId, CancellationToken ct)
        => HandleResult(await _mediator.Send(new DeleteBalanceDocumentCommand(id, documentId), ct));
}
