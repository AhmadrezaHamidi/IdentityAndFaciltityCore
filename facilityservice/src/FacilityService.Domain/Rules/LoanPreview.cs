namespace FacilityService.Domain.Rules;

public sealed record LoanPreview(int SelectedCount, long Total, long RemainingCredit, bool IsAllowed, int? MaxCoverablePeople,
    long? SuggestedMaxAmount, IReadOnlyList<Hint> Messages);
