using System.Text.RegularExpressions;

namespace FacilityService.Domain.Entity.Employees;

public class PersonnelHistory
{
    public long Id { get; private set; }
    public long PersonnelId { get; private set; }
    public PersonnelStatus? From { get; private set; }
    public PersonnelStatus To { get; private set; }
    public string? Reason { get; private set; }
    public string? Description { get; private set; }
    public Registrar By { get; private set; } = null!;
    public DateTime At { get; private set; }

    private PersonnelHistory() { }

    public PersonnelHistory(PersonnelStatus? from, PersonnelStatus to, string? reason, string? description, Registrar by, DateTime at)
    {
        From = from;
        To = to;
        Reason = reason;
        Description = description;
        By = by;
        At = at;
    }
}
