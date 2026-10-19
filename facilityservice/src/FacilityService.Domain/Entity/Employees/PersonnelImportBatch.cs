using System.Text.RegularExpressions;

namespace FacilityService.Domain.Entity.Employees;

/// <summary>نتیجهٔ بارگذاری گروهی (US-E2) برای GET Personnel/Imports/{id}.</summary>
public class PersonnelImportBatch
{
    public long Id { get; private set; }
    public long CompanyId { get; private set; }
    public string FileName { get; private set; } = null!;
    public int TotalRows { get; private set; }
    public int Accepted { get; private set; }
    public int Rejected { get; private set; }

    /// <summary>خطاها به‌صورت JSON: [{row, column, message}]</summary>
    public string ErrorsJson { get; private set; } = "[]";

    public Registrar Registrar { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private PersonnelImportBatch() { }

    public PersonnelImportBatch(long companyId, string fileName, Registrar by, DateTime at)
    {
        CompanyId = companyId;
        FileName = fileName;
        Registrar = by;
        CreatedAt = at;
    }

    public void Complete(int total, int accepted, string errorsJson)
    {
        TotalRows = total;
        Accepted = accepted;
        Rejected = total - accepted;
        ErrorsJson = errorsJson;
    }
}
