namespace FacilityService.Domain.Rules;

/// <summary>مبلغ یک درخواست ردنشده/لغونشده که از اعتبار طرح کم می‌شود، با تاریخ مبنای ثبتش.</summary>
public sealed record Allocation(int PlanId, long Amount, DateOnly BaseDate);
