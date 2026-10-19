using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Loans.ReadModels;

/// <summary>ستون‌های جدول نمونه: نوع تسهیلات، نرخ، مدت، سهم از میانگین، درصد قابل‌استفاده، اعتبار ایجادشده، تخصیص‌یافته، مانده.</summary>
/// <param name="ShareOfAverage">
/// مبلغ است، نه درصد — سهم ریالی این طرح از مجموع میانگین مبنا
/// (<c>totalAverageBase × usablePercent ÷ 100</c>، گرد‌شده). فرانت این را درصد نمایش می‌داد.
/// درصد همان <paramref name="UsablePercent"/> است.
/// </param>
/// <param name="UsablePercent">درصد قابل استفاده از میانگین برای این طرح (از جدول اطلاعات پایه).</param>
public sealed record PlanCreditDto(
    int PlanId, string Title, string ContractType, decimal Rate, int TermMonths,
    long ShareOfAverage, decimal UsablePercent, long CreatedCredit, long Allocated, long RemainingThisMonth);
