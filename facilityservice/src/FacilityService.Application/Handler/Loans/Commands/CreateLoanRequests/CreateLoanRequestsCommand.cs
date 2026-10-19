using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Loans.Commands.CreateLoanRequests;

/// <summary>ثبت درخواست تخصیص وام برای پرسنل انتخاب‌شده (US-D3). مجموع نباید از مانده اعتبار طرح بیشتر شود.</summary>
public sealed record CreateLoanRequestsCommand : FacilityBaseCommand<FacilityBaseMessageResponse<LoanBatchDto>>
{
    [JsonIgnore, BindNever] public long CompanyId { get; init; }

    /// <summary>فیلد ۱۴۳: از طرح‌های مصوب تفاهم‌نامهٔ شرکت</summary>
    [Required, Range(1, int.MaxValue)] public int PlanId { get; init; }

    /// <summary>فیلد ۱۴۷: فقط در بازهٔ یک ماه گذشته بر اساس روز افتتاح حساب (قاعدهٔ ۹)</summary>
    [Required] public DateOnly BaseDate { get; init; }

    [Required] public AllocationMode Mode { get; init; }

    /// <summary>فیلد ۲۴۹: در حالت یکسان؛ ۱۰٬۰۰۰٬۰۰۰ تا ۴٬۰۰۰٬۰۰۰٬۰۰۰ ریال</summary>
    [Range(10_000_000, 4_000_000_000)] public long? Amount { get; init; }

    /// <summary>حداقل ۱ نفر؛ فقط پرسنل تأییدشده</summary>
    [Required, MinLength(1), MaxLength(1000)] public IReadOnlyList<LoanRequestItemInput> Items { get; init; } = [];

    [StringLength(1000)] public string? Description { get; init; }
}
