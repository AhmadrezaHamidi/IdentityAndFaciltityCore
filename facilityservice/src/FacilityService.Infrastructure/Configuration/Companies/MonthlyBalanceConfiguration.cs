using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Companies;

public sealed class MonthlyBalanceConfiguration : IEntityTypeConfiguration<MonthlyBalance>
{
    public void Configure(EntityTypeBuilder<MonthlyBalance> e)
    {
        e.ToTable("MonthlyBalances", Schema);
        e.HasKey(x => new { x.CompanyId, x.Month });
        e.Property(x => x.Month).HasMaxLength(7).IsUnicode(false);
        RegistrarColumn(e.Property(x => x.EditedBy));
    }
}
