using FacilityService.Application.Common;

namespace FacilityService.Application.Handler.BasicInfo.Commands.DeleteScheme;

/// <summary>
/// حذف طرح. حذف نرم است و دو مانع دارد: آخرین طرح فعال حذف نمی‌شود (سند فرانت) و طرحی که
/// شرکت فعالی دارد هم نه — چون محاسبهٔ اعتبار آن شرکت‌ها از همین طرح خوانده می‌شود.
/// </summary>
public sealed record DeleteSchemeCommand(int SchemeId) : FacilityBaseCommand<FacilityBaseMessageResponse<bool>>;
