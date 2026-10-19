namespace FacilityService.Domain.Entity.Loans;

public class LoanRequestHistory
{
    public long Id { get; private set; }
    public long RequestId { get; private set; }
    public LoanRequestStatus? From { get; private set; }
    public LoanRequestStatus To { get; private set; }
    public PostApprovalStage? Stage { get; private set; }
    public string? Reason { get; private set; }
    public string? Description { get; private set; }
    public Registrar By { get; private set; } = null!;
    public DateTime At { get; private set; }

    private LoanRequestHistory() { }

    public LoanRequestHistory(LoanRequestStatus? from, LoanRequestStatus to, PostApprovalStage? stage, string? reason, string? description, Registrar by, DateTime at)
    {
        From = from;
        To = to;
        Stage = stage;
        Reason = reason;
        Description = description;
        By = by;
        At = at;
    }
}
