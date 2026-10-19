using System.Text.RegularExpressions;

namespace FacilityService.Domain.Entity.Companies;

public class MonthlyBalance
{
    public long CompanyId { get; private set; }

    /// <summary>ماه شمسی «1405/07»</summary>
    public string Month { get; private set; } = null!;

    public long Amount { get; private set; }
    public Registrar EditedBy { get; private set; } = null!;
    public DateTime EditedAt { get; private set; }

    private MonthlyBalance() { }

    public MonthlyBalance(string month, long amount, Registrar by, DateTime at)
    {
        Month = month;
        Set(amount, by, at);
    }

    public void Set(long amount, Registrar by, DateTime at)
    {
        Amount = amount;
        EditedBy = by;
        EditedAt = at;
    }
}
