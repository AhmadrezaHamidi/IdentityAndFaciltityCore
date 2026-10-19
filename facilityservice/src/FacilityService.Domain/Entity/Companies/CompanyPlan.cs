using System.Text.RegularExpressions;

namespace FacilityService.Domain.Entity.Companies;

public class CompanyPlan
{
    public long CompanyId { get; private set; }
    public int PlanId { get; private set; }

    private CompanyPlan() { }

    public CompanyPlan(int planId) => PlanId = planId;
}
