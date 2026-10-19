using System.Text.RegularExpressions;

namespace FacilityService.Domain.Entity.Companies;

/// <summary>مستند میانگین موجودی (US-B5).</summary>
public class BalanceDocument
{
    public long Id { get; private set; }
    public long CompanyId { get; private set; }
    public string? Month { get; private set; }
    public int DocumentTypeId { get; private set; }
    public string? Description { get; private set; }
    public long FileId { get; private set; }
    public StoredFile File { get; private set; } = null!;

    private BalanceDocument() { }

    public BalanceDocument(long companyId, string? month, int documentTypeId, string? description, StoredFile file)
    {
        CompanyId = companyId;
        Month = month;
        DocumentTypeId = documentTypeId;
        Description = description;
        File = file;
    }
}
