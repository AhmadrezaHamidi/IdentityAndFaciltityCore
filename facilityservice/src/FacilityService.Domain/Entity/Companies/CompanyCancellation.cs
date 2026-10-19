using System.Text.RegularExpressions;

namespace FacilityService.Domain.Entity.Companies;

public class CompanyCancellation
{
    public long CompanyId { get; private set; }
    public DateOnly CancelDate { get; private set; }
    public int ReasonId { get; private set; }
    public string ReasonTitle { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public long LetterFileId { get; private set; }
    public StoredFile LetterFile { get; private set; } = null!;
    public Registrar CancelledBy { get; private set; } = null!;
    public DateTime CancelledAt { get; private set; }

    private CompanyCancellation() { }

    public CompanyCancellation(DateOnly cancelDate, int reasonId, string reasonTitle, string description, StoredFile letterFile, Registrar by, DateTime at)
    {
        CancelDate = cancelDate;
        ReasonId = reasonId;
        ReasonTitle = reasonTitle;
        Description = description;
        LetterFile = letterFile;
        CancelledBy = by;
        CancelledAt = at;
    }
}
