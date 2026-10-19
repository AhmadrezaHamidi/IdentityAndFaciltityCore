namespace FacilityService.Domain.Rules;

public sealed record PlanCredit(
    SchemePlan Plan, long ShareOfAverage, long CreatedCredit, long Allocated, long RemainingThisMonth);
