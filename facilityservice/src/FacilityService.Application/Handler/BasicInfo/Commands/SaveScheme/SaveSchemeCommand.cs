using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Application.Handler.BasicInfo.Commands.SaveScheme;

/// <summary>
/// ایجاد یا ویرایش طرح — همهٔ ۱۴ مورد فرم در یک درخواست.
///
/// یک بدنه برای هر دو، چون فرم یکی است: در ایجاد <see cref="SchemeId"/> صفر است و
/// <see cref="Code"/> و <see cref="RowVersion"/> لازم‌اند/نیستند همان‌طور که زیر هر کدام نوشته شده.
/// قاعدهٔ «null یعنی تغییر نکند» تنظیمات حفظ شده است، ولی در ایجاد هر چیزی که نیاید
/// مقدار پیش‌فرض موجودیت را می‌گیرد.
/// </summary>
public sealed record SaveSchemeCommand : FacilityBaseCommand<FacilityBaseMessageResponse<SchemeFormDto>>
{
    /// <summary>در ویرایش از مسیر می‌آید؛ در ایجاد صفر است.</summary>
    [JsonIgnore, BindNever] public int SchemeId { get; init; }

    /// <summary>مورد ۱ فرم: نام طرح.</summary>
    [Required, StringLength(100, MinimumLength = 3)] public string Title { get; init; } = null!;

    /// <summary>کد طرح (NEGIN، SHAJARE، ...). فقط در ایجاد؛ در ویرایش نادیده گرفته می‌شود چون کلید ارجاع است.</summary>
    [StringLength(30, MinimumLength = 2)] public string? Code { get; init; }

    /// <summary>طرح غیرفعال در کشویی‌ها نمی‌آید ولی شرکت‌های موجودش دست‌نخورده می‌مانند.</summary>
    public bool IsActive { get; init; } = true;

    // ── موارد ۲ تا ۱۰ فرم: همان تنظیمات اطلاعات پایه ──

    [MinLength(1), MaxLength(2)] public IReadOnlyList<ContractType>? ContractTypes { get; init; }

    /// <summary>مورد ۲: نرخ قرض‌الحسنه، ۰ تا ۴ درصد.</summary>
    [Range(0, 4)] public decimal? QardHasanRate { get; init; }

    /// <summary>مورد ۸: نرخ‌های مرابحه، حداکثر ۵ نرخ، هرکدام ۰ تا ۲۳ درصد.</summary>
    [MaxLength(5)] public IReadOnlyList<decimal>? MurabahaRates { get; init; }

    /// <summary>مورد ۹: مدت‌های بازپرداخت، مضرب ۳ از ۳ تا ۶۰ ماه.</summary>
    [MaxLength(20)] public IReadOnlyList<int>? RepaymentTermsMonths { get; init; }

    /// <summary>مورد ۳: ضریب تأثیر «حقوق‌بگیر بانک سپه = بله».</summary>
    [Range(0.01, 10)] public decimal? SalaryFromSepahMultiplierYes { get; init; }

    [Range(0.01, 10)] public decimal? SalaryFromSepahMultiplierNo { get; init; }

    /// <summary>مورد ۴: مدت قرارداد (ماه).</summary>
    [Range(1, 120)] public int? ContractDurationMonths { get; init; }

    /// <summary>مورد ۵: مهلت استفاده از مانده اعتبار پس از انقضا (ماه).</summary>
    [Range(0, 120)] public int? GraceMonthsAfterExpiry { get; init; }

    /// <summary>مورد ۶: حداکثر تعداد طرح قابل انتخاب برای هر شرکت.</summary>
    [Range(1, 10)] public int? MaxPlansPerCompany { get; init; }

    /// <summary>مورد ۷: مهلت مجاز شدن معرفی پرسنل پس از انعقاد قرارداد (ماه).</summary>
    [Range(0, 120)] public int? PersonnelIntroductionDelayMonths { get; init; }

    [Range(0, 180)] public int? WaitingPeriodDays { get; init; }

    public bool? DigitalApprovalEnabled { get; init; }

    /// <summary>مورد ۱۰: جدول درصد قابل استفاده از میانگین (هر مدت × قرض‌الحسنه و هر نرخ مرابحه).</summary>
    public IReadOnlyList<UsablePercentDto>? UsableTable { get; init; }

    // ── موارد ۱۱ تا ۱۴ فرم: فهرست‌های همین طرح ──
    // عنوان‌ها فرستاده می‌شوند، نه شناسه‌ها: فرانت فهرست را همان‌طور که کاربر چیده می‌فرستد و
    // بک‌اند تطبیق می‌دهد — عنوان موجود نگه داشته می‌شود (تا رکوردهای قدیمی‌اش نشکنند)، تازه
    // اضافه و برداشته‌شده غیرفعال می‌شود. null یعنی این فهرست تغییر نکند.

    /// <summary>مورد ۱۱: دلایل لغو قرارداد.</summary>
    [MaxLength(100)] public IReadOnlyList<string>? ContractCancelReasons { get; init; }

    /// <summary>مورد ۱۲: دلایل رد درخواست وام.</summary>
    [MaxLength(100)] public IReadOnlyList<string>? LoanRejectReasons { get; init; }

    /// <summary>مورد ۱۳: دلایل عدم اعطای تسهیلات غیرحضوری.</summary>
    [MaxLength(100)] public IReadOnlyList<string>? RemoteGrantDeniedReasons { get; init; }

    /// <summary>مورد ۱۴: انواع مستند میانگین موجودی.</summary>
    [MaxLength(100)] public IReadOnlyList<string>? DocumentTypes { get; init; }

    /// <summary>دلایل لغو درخواست وام (در فهرست سند نبود ولی سرویس لغو درخواست لازمش دارد).</summary>
    [MaxLength(100)] public IReadOnlyList<string>? LoanCancelReasons { get; init; }

    /// <summary>دلایل رد پرسنل معرفی‌شده.</summary>
    [MaxLength(100)] public IReadOnlyList<string>? PersonnelRejectReasons { get; init; }

    /// <summary>در ویرایش الزامی (قفل خوش‌بینانه)؛ در ایجاد خالی.</summary>
    public string? RowVersion { get; init; }
}
