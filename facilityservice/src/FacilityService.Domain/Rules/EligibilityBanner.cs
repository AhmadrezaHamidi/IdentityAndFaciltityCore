namespace FacilityService.Domain.Rules;

public sealed record EligibilityBanner(EligibilityStatus Status, string Message, int? AllowedDayOfMonth);
