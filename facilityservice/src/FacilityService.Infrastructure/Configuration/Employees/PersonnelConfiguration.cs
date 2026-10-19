using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Employees;

public sealed class PersonnelConfiguration : IEntityTypeConfiguration<Personnel>
{
    public void Configure(EntityTypeBuilder<Personnel> e)
    {
        e.ToTable("Personnel", Schema);
        Versioned(e);
        e.Property(x => x.FullName).HasMaxLength(60).IsRequired();
        e.Property(x => x.NationalCode).HasMaxLength(10).IsUnicode(false).IsRequired();
        e.Property(x => x.Mobile).HasMaxLength(11).IsUnicode(false).IsRequired();
        e.Property(x => x.PersonnelCode).HasMaxLength(20).IsUnicode(false).IsRequired();
        e.Property(x => x.RejectReason).HasMaxLength(200);
        e.Property(x => x.DeleteDescription).HasMaxLength(500);
        RegistrarColumn(e.Property(x => x.Registrar));
        e.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne(x => x.DeleteFile).WithMany().HasForeignKey(x => x.DeleteFileId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<PersonnelImportBatch>().WithMany().HasForeignKey(x => x.ImportBatchId).OnDelete(DeleteBehavior.Restrict);
        e.HasMany(x => x.History).WithOne().HasForeignKey(x => x.PersonnelId);
        e.HasIndex(x => new { x.CompanyId, x.Status, x.Priority });
        e.HasIndex(x => new { x.CompanyId, x.NationalCode });
    }
}
