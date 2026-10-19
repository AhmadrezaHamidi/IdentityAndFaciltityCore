using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Loans.ReadModels;

/// <param name="Banner">بنر رنگی بالای تب</param>
/// <param name="TotalAverageBase">مجموع میانگین مبنای محاسبهٔ اعتبار (آخرین میانگین‌های مثبت)</param>
/// <param name="BaseDateDefault">تاریخ مبنای پیش‌فرض فرم ثبت وام (امروز).</param>
/// <param name="BaseDateMin">
/// کمینهٔ تاریخ مبنای مجاز: یک ماه شمسی قبل از امروز. فرانت تا امروز این بازه را نمی‌دانست و
/// خطا فقط هنگام ثبت معلوم می‌شد؛ حالا می‌تواند تاریخ‌شمار را همین‌جا محدود کند.
/// </param>
/// <param name="BaseDateMax">بیشینهٔ تاریخ مبنای مجاز: امروز. تاریخ آینده پذیرفته نمی‌شود.</param>
public sealed record CompanyCreditDto(
    long CompanyId, DateOnly BaseDate, EligibilityBannerDto Banner,
    long MonthRemainingCredit, long TotalAverageBase, long? CurrentMonthAverage,
    IReadOnlyList<PlanCreditDto> Plans,
    DateOnly BaseDateDefault, DateOnly BaseDateMin, DateOnly BaseDateMax);
