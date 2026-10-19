using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.BasicInfo;

public sealed class SchemePlanConfiguration : IEntityTypeConfiguration<SchemePlan>
{
    public void Configure(EntityTypeBuilder<SchemePlan> e)
    {
        e.ToTable("SchemePlans", Schema);
        e.Property(x => x.Rate).HasPrecision(5, 2);
        e.Property(x => x.UsablePercent).HasPrecision(5, 2);
        e.HasIndex(x => new { x.SchemeId, x.ContractType, x.Rate, x.TermMonths }).IsUnique();
        e.Ignore(x => x.Title);
        e.Ignore(x => x.ContractTitle);
    }
}
