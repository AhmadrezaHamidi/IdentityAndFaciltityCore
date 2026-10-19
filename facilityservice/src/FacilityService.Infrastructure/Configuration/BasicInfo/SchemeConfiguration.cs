using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.BasicInfo;

public sealed class SchemeConfiguration : IEntityTypeConfiguration<Scheme>
{
    public void Configure(EntityTypeBuilder<Scheme> e)
    {
        e.ToTable("Schemes", Schema);
        e.Property(x => x.Code).HasMaxLength(30).IsRequired();
        e.Property(x => x.Title).HasMaxLength(100).IsRequired();
        e.HasIndex(x => x.Code).IsUnique();
    }
}
