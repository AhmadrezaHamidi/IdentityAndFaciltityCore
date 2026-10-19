using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Commands.CreateCompany;

/// <summary>US-B2 — ثبت شرکت. کد شعبهٔ شرکت = کد شعبهٔ کاربر ثبت‌کننده (ادمین می‌تواند BranchCode بدهد).</summary>
public record CreateCompanyCommand : FacilityBaseCommand<FacilityBaseMessageResponse<CompanyDetailsDto>>
{
    [Required, Range(1, int.MaxValue)] public int SchemeId { get; init; }

    /// <summary>فیلد ۲۱۷: ۳ تا ۱۰۰ حرف فارسی، فاصله و عدد</summary>
    [Required, StringLength(100, MinimumLength = 3)]
    [RegularExpression(@"^[؀-ۿ‌\s0-9۰-۹]+$", ErrorMessage = "نام شرکت فقط حروف فارسی، فاصله و عدد")]
    public string Name { get; init; } = null!;

    /// <summary>فیلد ۲۱۸: شناسهٔ ملی دقیقاً ۱۱ رقم</summary>
    [Required, RegularExpression(@"^\d{11}$", ErrorMessage = "شناسهٔ ملی ۱۱ رقم است")]
    public string NationalId { get; init; } = null!;

    /// <summary>فیلد ۲۱۹: شمارهٔ تفاهم‌نامه ۳ تا ۵۰؛ حرف، عدد، / - .</summary>
    [Required, StringLength(50, MinimumLength = 3), RegularExpression(@"^[\p{L}0-9/\-.]+$")]
    public string AgreementNumber { get; init; } = null!;

    /// <summary>فیلد ۲۲۰: شمارهٔ حساب جاری ۸ تا ۲۴ رقم</summary>
    [Required, RegularExpression(@"^\d{8,24}$")]
    public string AccountNumber { get; init; } = null!;

    /// <summary>فیلد ۲۲۱: تاریخ انعقاد قرارداد (شمسی ۱۴۰۰/۰۱/۰۱ تا ۱۴۵۰/۱۲/۲۹)</summary>
    [Required] public DateOnly ContractDate { get; init; }

    /// <summary>فیلد ۲۲۲: روز افتتاح حساب سپرده؛ روز ماهِ این تاریخ «روز مجاز انتقال اعتبار» است</summary>
    [Required] public DateOnly AccountOpeningDate { get; init; }

    /// <summary>فیلد ۲۳۱: دریافت حقوق پرسنل از بانک سپه</summary>
    public bool SalaryFromSepah { get; init; }

    /// <summary>طرح‌های مندرج در تفاهم‌نامه؛ حداقل ۱ و حداکثر سقف اطلاعات پایه</summary>
    // = new List — نه [] : آرایهٔ ثابت‌اندازه جلوی Model Binding فرم (multipart) را می‌گیرد و مقدار خالی می‌ماند.
    [Required, MinLength(1)] public IReadOnlyList<int> PlanIds { get; init; } = new List<int>();

    /// <summary>فقط ادمین/دیجیتال؛ برای کاربر شعبه از پروفایل خودش پر می‌شود.</summary>
    [StringLength(50)] public string? BranchCode { get; init; }

    /// <summary>فیلد ۲۲۳: فایل تفاهم‌نامه (pdf, jpg, jpeg, png, zip تا ۵ مگابایت). کنترلر از فرم پر می‌کند.</summary>
    [JsonIgnore, BindNever] public FileUpload? AgreementFile { get; init; }
}
