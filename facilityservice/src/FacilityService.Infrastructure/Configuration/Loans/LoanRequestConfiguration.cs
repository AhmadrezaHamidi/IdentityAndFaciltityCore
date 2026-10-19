using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Loans;

public sealed class LoanRequestConfiguration : IEntityTypeConfiguration<LoanRequest>
{
    public void Configure(EntityTypeBuilder<LoanRequest> e)
    {
        e.ToTable("LoanRequests", Schema);
        Versioned(e);
        e.Property(x => x.BranchCode).HasMaxLength(50).IsUnicode(false).IsRequired();
        e.Property(x => x.Description).HasMaxLength(1200);
        RegistrarColumn(e.Property(x => x.Registrar));
        e.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<Personnel>().WithMany().HasForeignKey(x => x.PersonnelId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<SchemePlan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
        e.HasMany(x => x.History).WithOne().HasForeignKey(x => x.RequestId);
        e.HasMany(x => x.Documents).WithOne().HasForeignKey(x => x.RequestId);
        e.HasIndex(x => new { x.Status, x.BranchCode });
        e.HasIndex(x => new { x.CompanyId, x.Status, x.PlanId });
        e.HasIndex(x => new { x.PersonnelId, x.Status });
    }
}
