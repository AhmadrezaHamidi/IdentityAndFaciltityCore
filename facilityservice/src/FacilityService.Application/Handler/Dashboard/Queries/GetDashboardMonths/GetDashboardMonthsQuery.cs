using FacilityService.Application.Common;
using FacilityService.Contracts.Dashboard.ReadModels;

namespace FacilityService.Application.Handler.Dashboard.Queries.GetDashboardMonths;

/// <summary>
/// ماه‌هایی که داشبورد برایشان داده دارد، به ترتیب (تازه‌ترین اول).
///
/// چرا لازم شد: فیلتر ماه رتبه‌بندی سرویس نداشت و قالب کلید ماه هم اعلام نمی‌شد، پس فرانت
/// نمی‌دانست چه چیزی در <c>GET Dashboard/CompanyRanking?month=</c> بفرستد یا ستون‌های
/// «عملکرد ماهانه» را به چه ترتیبی بچیند.
/// </summary>
public sealed record GetDashboardMonthsQuery : FacilityBaseQuery<FacilityBaseMessageResponse<IReadOnlyList<MonthOptionDto>>>;
