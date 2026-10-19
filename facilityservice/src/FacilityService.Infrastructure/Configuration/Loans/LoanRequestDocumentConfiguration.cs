using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Loans;

public sealed class LoanRequestDocumentConfiguration : IEntityTypeConfiguration<LoanRequestDocument>
{
    public void Configure(EntityTypeBuilder<LoanRequestDocument> e)
    {
        e.ToTable("LoanRequestDocuments", Schema);
        e.HasKey(x => new { x.RequestId, x.FileId });
        e.HasOne(x => x.File).WithMany().HasForeignKey(x => x.FileId).OnDelete(DeleteBehavior.Restrict);
    }
}
