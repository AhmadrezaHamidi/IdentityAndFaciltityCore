using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.Dashboard.ReadModels;

/// <param name="RemovalKind">
/// علت خروج شرکت از فهرست فعال: لغو قرارداد (۲)، حذف شرکت (۴) یا انقضای قرارداد (۳).
/// فرانت این سه را جدا نمایش می‌دهد و تا امروز نمی‌توانست تشخیص دهد — شرکت‌های لغوشده هم
/// در همین فهرست هستند.
/// </param>
/// <param name="ApprovalRate">
/// برای شرکتی که هیچ درخواستی ندارد null است، نه صفر؛ فرانت در این حالت «—» نشان می‌دهد
/// و صفر یعنی «درخواست داشت و هیچ‌کدام تأیید نشد».
/// </param>
public sealed record RemovedCompanyDto(long CompanyId, string CompanyName, DateOnly RemovedAt, long? AverageBalance,
    long AllocatedTotal, int RequestCount, decimal? ApprovalRate, CompanyStatus RemovalKind);
