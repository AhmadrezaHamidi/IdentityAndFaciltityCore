using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.BasicInfo;

public sealed class ReasonConfiguration : IEntityTypeConfiguration<Reason>
{
    public void Configure(EntityTypeBuilder<Reason> e)
    {
        e.ToTable("Reasons", Schema);
        e.Property(x => x.Title).HasMaxLength(100).IsRequired();
        e.HasIndex(x => new { x.Type, x.IsActive, x.SortOrder });
    }
}
