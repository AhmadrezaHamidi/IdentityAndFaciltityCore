using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Loans.ReadModels;

public sealed record LoanRequestItemInput
{
    [Required, Range(1, long.MaxValue)] public long PersonnelId { get; init; }

    /// <summary>فیلد ۱۵۹: در حالت متغیر الزامی؛ ۱۰٬۰۰۰٬۰۰۰ تا ۴٬۰۰۰٬۰۰۰٬۰۰۰ ریال</summary>
    [Range(10_000_000, 4_000_000_000)] public long? Amount { get; init; }

    /// <summary>فیلد ۱۶۱: ترتیب پرداخت ۱ تا ۹۹۹۹ (پیش‌فرض اولویت پرسنل)</summary>
    [Range(1, 9_999)] public int? PaymentOrder { get; init; }
}
