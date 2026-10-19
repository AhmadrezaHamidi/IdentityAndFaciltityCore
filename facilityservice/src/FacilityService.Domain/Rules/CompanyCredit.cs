namespace FacilityService.Domain.Rules;

/// <param name="UsedLastMonth">شرکت ماه قبل وام تخصیص داده؛ مانده به اعتبار ماه جاری محدود است</param>
/// <param name="ContractYearStart">شروع سال جاری قرارداد (تمدید خودکار)</param>
public sealed record CompanyCredit(
    DateOnly BaseDate, EligibilityBanner Banner, long MonthRemainingCredit, long TotalAverageBase, long? CurrentMonthAverage,
    IReadOnlyList<PlanCredit> Plans, bool UsedLastMonth, DateOnly ContractYearStart);
