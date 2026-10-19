using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Companies;

public sealed class BalanceDocumentConfiguration : IEntityTypeConfiguration<BalanceDocument>
{
    public void Configure(EntityTypeBuilder<BalanceDocument> e)
    {
        e.ToTable("BalanceDocuments", Schema);
        e.Property(x => x.Month).HasMaxLength(7).IsUnicode(false);
        e.Property(x => x.Description).HasMaxLength(500);
        e.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<Reason>().WithMany().HasForeignKey(x => x.DocumentTypeId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne(x => x.File).WithMany().HasForeignKey(x => x.FileId).OnDelete(DeleteBehavior.Restrict);
        e.HasIndex(x => new { x.CompanyId, x.Month });
    }
}
