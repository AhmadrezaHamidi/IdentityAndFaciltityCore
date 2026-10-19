using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Employees;

public sealed class PersonnelHistoryConfiguration : IEntityTypeConfiguration<PersonnelHistory>
{
    public void Configure(EntityTypeBuilder<PersonnelHistory> e)
    {
        e.ToTable("PersonnelHistory", Schema);
        e.Property(x => x.Reason).HasMaxLength(200);
        e.Property(x => x.Description).HasMaxLength(1000);
        RegistrarColumn(e.Property(x => x.By));
    }
}
