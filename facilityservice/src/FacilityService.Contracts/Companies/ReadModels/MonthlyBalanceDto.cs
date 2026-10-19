using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Companies.ReadModels;

/// <param name="Month">ماه شمسی به‌صورت «۱۴۰۵/۰۷»</param>
/// <param name="RemainingCredit">مانده اعتبار تا این ماه (محاسبه‌شده از وام‌های تخصیص‌یافته)</param>
/// <param name="HasDocuments">false ← هینت «برای این میانگین مستندی پیوست نشده است»</param>
/// <param name="UsedCredit">اعتبار مصرف‌شده: مجموع درخواست‌های ردنشده/لغونشده با تاریخ مبنا در همین ماه (معیار پذیرش ۱ مدیریت شرکت‌ها)</param>
public sealed record MonthlyBalanceDto(string Month, long? AverageBalance, long? RemainingCredit, int DocumentCount, bool HasDocuments,
    RegistrarDto? LastEditedBy, DateTime? LastEditedAt, long UsedCredit);
