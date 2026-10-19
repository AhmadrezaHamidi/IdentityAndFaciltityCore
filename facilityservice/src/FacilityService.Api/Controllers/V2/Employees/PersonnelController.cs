using FacilityService.Api.Base;
using FacilityService.Api.Attributes;
using FacilityService.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Api.Controllers.V2.Employees;

/// <summary>تب «پرسنل شرکت». جایگزین BulkUserImport/UploadExel که بدون چک مجوز بود.</summary>
[ApiVersion("2")]
[SwaggerTag("پرسنل شرکت")]
public class PersonnelController(ILogger<PersonnelController> logger) : FacilityPanelControllerBase(logger)
{
    [HttpGet("Companies/{companyId:long}/Personnel")]
    [RequireFacilityPermission(K.PersonnelView, K.LoansCreate)]
    [SwaggerOperation(OperationId = "Personnel_Search", Summary = "پرسنل یک شرکت",
        Description = "جستجو در نام، کد ملی، موبایل، کد پرسنلی + بازهٔ تاریخ استخدام. مرتب بر اساس اولویت. " +
                      "در انتظار/ردشده فقط برای ادمین، منابع انسانی و مدیر همان شرکت.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<FacilityPagedResponse<PersonnelListItemDto>>))]
    public async Task<IActionResult> Search(long companyId, [FromQuery] SearchPersonnelQuery query, CancellationToken ct)
        => HandleResult(await _mediator.Send(query with { CompanyId = companyId }, ct));

    [HttpGet("Personnel/{id:long}")]
    [RequireFacilityPermission(K.PersonnelView)]
    [SwaggerOperation(OperationId = "Personnel_Get", Summary = "جزئیات و تاریخچهٔ وضعیت (US-E3)")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<PersonnelDetailsDto>))]
    public async Task<IActionResult> Get(long id, CancellationToken ct) => HandleResult(await _mediator.Send(new GetPersonnelQuery(id), ct));

    [HttpPost("Companies/{companyId:long}/Personnel")]
    [RequireFacilityPermission(K.PersonnelCreate)]
    [SwaggerOperation(OperationId = "Personnel_Create", Summary = "افزودن پرسنل", Description = "با وضعیت «در انتظار تأیید مدیر شرکت» ثبت می‌شود.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<PersonnelDetailsDto>))]
    public async Task<IActionResult> Create(long companyId, [FromBody] CreatePersonnelCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { CompanyId = companyId }, ct));

    [HttpPut("Personnel/{id:long}")]
    [RequireFacilityPermission(K.PersonnelEdit)]
    [SwaggerOperation(OperationId = "Personnel_Update", Summary = "ویرایش یا اصلاح و ارسال دوباره",
        Description = "برای فرد ردشده وضعیت دوباره «در انتظار تأیید مدیر» می‌شود. فرد استفاده‌شده در وام قفل است.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<PersonnelDetailsDto>))]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePersonnelCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { Id = id }, ct));

    [HttpPost("Personnel/{id:long}/Delete")]
    [Consumes("multipart/form-data")]
    [RequireFacilityPermission(K.PersonnelDelete)]
    [SwaggerOperation(OperationId = "Personnel_Delete", Summary = "حذف پرسنل", Description = "توضیح و فایل الزامی. POST چون فایل دارد.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<bool>))]
    public async Task<IActionResult> Delete(long id, [FromForm] DeletePersonnelCommand command, IFormFile? file, CancellationToken ct)
        => HandleResult(await _mediator.Send(command with { Id = id, File = await file.ToUploadAsync(ct) }, ct));

    [HttpGet("Personnel/Import/Template")]
    [RequireFacilityPermission(K.PersonnelUpload)]
    [SwaggerOperation(OperationId = "Personnel_ImportTemplate", Summary = "دانلود نمونه فایل اکسل")]
    public async Task<IActionResult> Template(CancellationToken ct) => FileResult(await _mediator.Send(new GetPersonnelImportTemplateQuery(), ct));

    [HttpPost("Companies/{companyId:long}/Personnel/Import")]
    [Consumes("multipart/form-data")]
    [RequireFacilityPermission(K.PersonnelUpload)]
    [SwaggerOperation(OperationId = "Personnel_Import", Summary = "بارگذاری گروهی (US-E2)", Description = "xlsx, xls, csv تا ۱۰ مگابایت. خطای هر ردیف برمی‌گردد.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<PersonnelImportResultDto>))]
    public async Task<IActionResult> Import(long companyId, IFormFile? file, CancellationToken ct)
        => HandleResult(await _mediator.Send(new ImportPersonnelCommand { CompanyId = companyId, File = await file.ToUploadAsync(ct) }, ct));

    [HttpGet("Personnel/Import/{batchId:long}")]
    [RequireFacilityPermission(K.PersonnelUpload)]
    [SwaggerOperation(OperationId = "Personnel_ImportResult", Summary = "نتیجهٔ یک بارگذاری")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<PersonnelImportResultDto>))]
    public async Task<IActionResult> ImportResult(long batchId, CancellationToken ct) => HandleResult(await _mediator.Send(new GetPersonnelImportQuery(batchId), ct));

    [HttpPost("Personnel/Decisions")]
    [RequireFacilityPermission(K.PersonnelApprove)]
    [SwaggerOperation(OperationId = "Personnel_Decide", Summary = "تأیید/رد افراد معرفی‌شده (US-D1)", Description = "تکی یا گروهی، در یک تراکنش.")]
    [SwaggerResponse(200, null, typeof(FacilityBaseMessageResponse<int>))]
    public async Task<IActionResult> Decide([FromBody] DecidePersonnelCommand command, CancellationToken ct)
        => HandleResult(await _mediator.Send(command, ct));
}
