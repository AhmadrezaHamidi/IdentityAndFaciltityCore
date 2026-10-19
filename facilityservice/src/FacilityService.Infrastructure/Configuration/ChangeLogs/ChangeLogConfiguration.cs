using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.ChangeLogs;

public sealed class ChangeLogConfiguration : IEntityTypeConfiguration<ChangeLog>
{
    public void Configure(EntityTypeBuilder<ChangeLog> e)
    {
        e.ToTable("ChangeLogs", Schema);
        e.Property(x => x.Action).HasMaxLength(100).IsRequired();
        e.Property(x => x.Details).HasMaxLength(2000).IsRequired();
        e.Property(x => x.BranchCode).HasMaxLength(50).IsUnicode(false);
        e.Property(x => x.RestoreKind).HasMaxLength(50).IsUnicode(false);
        RegistrarColumn(e.Property(x => x.User));
        e.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
        e.HasIndex(x => new { x.CompanyId, x.Id });
        e.HasIndex(x => new { x.BranchCode, x.Id });
        e.HasIndex(x => x.At);
        e.Ignore(x => x.CanRestore);
    }
}
