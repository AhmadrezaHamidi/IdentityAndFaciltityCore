using FacilityService.Application.Common;
using FacilityService.Contracts.Common;

namespace FacilityService.Application.Handler.Lookups.Queries.GetEnumCatalog;

/// <summary>
/// عنوان همهٔ enumها. فرانت یک بار صدا می‌زند و کش می‌کند.
///
/// چرا لازم شد: وضعیت درخواست وام ۸ مقدار عددی است، وضعیت شرکت ۴، وضعیت پرسنل ۴،
/// مرحلهٔ پس از تأیید ۴، دستهٔ لاگ ۴، و هیچ‌کدام عنوان نداشتند. فرانت ناچار بود
/// عنوان‌ها را در کد خودش تکرار کند، که یعنی افزودن یک وضعیت در بک‌اند، ستون وضعیت
/// را در فرانت خالی می‌گذاشت.
/// </summary>
public sealed record GetEnumCatalogQuery : FacilityBaseQuery<FacilityBaseMessageResponse<EnumCatalogDto>>;
