using FacilityService.Application.Common;

namespace FacilityService.Application.Handler.BasicInfo.Queries.GetScheme;

/// <summary>همهٔ ۱۴ مورد فرم طرح در یک پاسخ: هویت طرح، تنظیمات و فهرست‌های دلایلش.</summary>
public sealed record GetSchemeQuery(int SchemeId) : FacilityBaseQuery<FacilityBaseMessageResponse<SchemeFormDto>>;
