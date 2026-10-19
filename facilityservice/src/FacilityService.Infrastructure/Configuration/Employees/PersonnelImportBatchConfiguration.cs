using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Employees;

public sealed class PersonnelImportBatchConfiguration : IEntityTypeConfiguration<PersonnelImportBatch>
{
    public void Configure(EntityTypeBuilder<PersonnelImportBatch> e)
    {
        e.ToTable("PersonnelImports", Schema);
        e.Property(x => x.FileName).HasMaxLength(260).IsRequired();
        e.Property(x => x.ErrorsJson).IsRequired();
        RegistrarColumn(e.Property(x => x.Registrar));
        e.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
    }
}
