using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Loans;

public sealed class LoanBatchConfiguration : IEntityTypeConfiguration<LoanBatch>
{
    public void Configure(EntityTypeBuilder<LoanBatch> e)
    {
        e.ToTable("LoanBatches", Schema);
        e.Property(x => x.BranchCode).HasMaxLength(50).IsUnicode(false).IsRequired();
        e.Property(x => x.Description).HasMaxLength(1000);
        RegistrarColumn(e.Property(x => x.Registrar));
        e.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<SchemePlan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
        e.HasMany(x => x.Requests).WithOne().HasForeignKey(x => x.BatchId);
    }
}
