using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Companies;

public sealed class CompanyCancellationConfiguration : IEntityTypeConfiguration<CompanyCancellation>
{
    public void Configure(EntityTypeBuilder<CompanyCancellation> e)
    {
        e.ToTable("CompanyCancellations", Schema);
        e.HasKey(x => x.CompanyId);
        e.Property(x => x.ReasonTitle).HasMaxLength(100).IsRequired();
        e.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        RegistrarColumn(e.Property(x => x.CancelledBy));
        e.HasOne<Reason>().WithMany().HasForeignKey(x => x.ReasonId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne(x => x.LetterFile).WithMany().HasForeignKey(x => x.LetterFileId).OnDelete(DeleteBehavior.Restrict);
    }
}
