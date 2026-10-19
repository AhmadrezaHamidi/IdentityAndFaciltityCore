using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Commands.UpdateBasicInfo;

/// <summary>ذخیرهٔ کامل تنظیمات طرح (دکمهٔ «ذخیرهٔ تنظیمات پایه»). پر کردن همهٔ موارد الزامی نیست (نکتهٔ سند).</summary>
public sealed record UpdateBasicInfoCommand : FacilityBaseCommand<FacilityBaseMessageResponse<BasicInfoDto>>
{
    [JsonIgnore, BindNever] public int SchemeId { get; init; }

    /// <summary>فیلد ۲۲: حداقل ۱، حداکثر ۲ (قرض‌الحسنه، مرابحه)</summary>
    [MinLength(1), MaxLength(2)] public IReadOnlyList<ContractType>? ContractTypes { get; init; }

    /// <summary>فیلد ۲۳۲: نرخ قرض‌الحسنه، ۰ تا ۴ درصد</summary>
    [Range(0, 4)] public decimal? QardHasanRate { get; init; }

    /// <summary>فیلد ۳۰: نرخ‌های مرابحه (درصد)، هرکدام ۰ تا ۲۳؛ نمونهٔ پنل ۵ نرخ. null = تغییر نکند</summary>
    [MaxLength(5)] public IReadOnlyList<decimal>? MurabahaRates { get; init; }

    /// <summary>فیلد ۱۹۶: مدت‌های بازپرداخت، مضرب ۳ از ۳ تا ۶۰ ماه</summary>
    [MaxLength(20)] public IReadOnlyList<int>? RepaymentTermsMonths { get; init; }

    /// <summary>فیلد ۲۳۰: ضریب حقوق از سپه (بله)، ۰٫۰۱ تا ۱۰ با دو رقم اعشار</summary>
    [Range(0.01, 10)] public decimal? SalaryFromSepahMultiplierYes { get; init; }

    [Range(0.01, 10)] public decimal? SalaryFromSepahMultiplierNo { get; init; }

    /// <summary>فیلد ۲۲۹: مدت قرارداد ۱ تا ۱۲۰ ماه</summary>
    [Range(1, 120)] public int? ContractDurationMonths { get; init; }

    /// <summary>مهلت استفاده از مانده اعتبار پس از انقضا (ماه) — نمونهٔ پنل</summary>
    [Range(0, 120)] public int? GraceMonthsAfterExpiry { get; init; }

    /// <summary>فیلد ۲۲۸: حداکثر تعداد طرح قابل انتخاب ۱ تا ۱۰</summary>
    [Range(1, 10)] public int? MaxPlansPerCompany { get; init; }

    /// <summary>مهلت مجاز شدن معرفی پرسنل پس از انعقاد قرارداد (ماه) — نمونهٔ پنل</summary>
    [Range(0, 120)] public int? PersonnelIntroductionDelayMonths { get; init; }

    /// <summary>فیلد ۲۲۷: دورهٔ انتظار معرفی به بانک ۰ تا ۱۸۰ روز</summary>
    [Range(0, 180)] public int? WaitingPeriodDays { get; init; }

    /// <summary>مرحلهٔ «تأیید شعبهٔ دیجیتال» بعد از تأیید اعتبارات ⚠ Q</summary>
    public bool? DigitalApprovalEnabled { get; init; }

    /// <summary>فیلد ۱۹۷: درصد قابل استفاده ۰٫۱ تا ۱۰۰ با دو رقم اعشار. هر ردیف یک طرح؛ ردیف حذف‌شده غیرفعال می‌شود. null = تغییر نکند</summary>
    public IReadOnlyList<UsablePercentDto>? UsableTable { get; init; }

    [Required] public string RowVersion { get; init; } = null!;
}
