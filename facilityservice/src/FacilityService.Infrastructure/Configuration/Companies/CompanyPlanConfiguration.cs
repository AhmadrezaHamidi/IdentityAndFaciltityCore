using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Companies;

public sealed class CompanyPlanConfiguration : IEntityTypeConfiguration<CompanyPlan>
{
    public void Configure(EntityTypeBuilder<CompanyPlan> e)
    {
        e.ToTable("CompanyPlans", Schema);
        e.HasKey(x => new { x.CompanyId, x.PlanId });
        e.HasOne<SchemePlan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
    }
}
