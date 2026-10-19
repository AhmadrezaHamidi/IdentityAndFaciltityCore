namespace FacilityService.Domain.Entity.Loans;

/// <summary>پیوست تصمیم (مصوبه) روی درخواست.</summary>
public class LoanRequestDocument
{
    public long RequestId { get; private set; }
    public long FileId { get; private set; }
    public StoredFile File { get; private set; } = null!;

    private LoanRequestDocument() { }

    public LoanRequestDocument(StoredFile file) => File = file;
}
