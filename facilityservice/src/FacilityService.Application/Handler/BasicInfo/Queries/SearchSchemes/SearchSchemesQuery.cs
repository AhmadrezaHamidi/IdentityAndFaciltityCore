using System.ComponentModel.DataAnnotations;
using FacilityService.Application.Common;

namespace FacilityService.Application.Handler.BasicInfo.Queries.SearchSchemes;

/// <summary>
/// جدول «اطلاعات پایه طرح»، صفحه‌بندی‌شده و با همان ستون‌هایی که نمایش داده می‌شود.
///
/// <c>GET BasicInfo/Schemes</c> قبلی فقط شناسه، کد، عنوان و فعال بودن می‌داد و برای پر کردن
/// جدول باید برای هر ردیف سرویس جزئیات صدا زده می‌شد.
/// </summary>
public sealed record SearchSchemesQuery : FacilityPagedQuery<FacilityBaseMessageResponse<FacilityPagedResponse<SchemeListItemDto>>>
{
    /// <summary>پیش‌فرض فقط طرح‌های فعال. غیرفعال‌ها را فقط کسی می‌بیند که اطلاعات پایه را ویرایش می‌کند.</summary>
    public bool IncludeInactive { get; init; }
}
