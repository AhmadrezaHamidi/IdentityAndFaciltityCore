using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.BasicInfo;

public sealed class SchemeSettingsConfiguration : IEntityTypeConfiguration<SchemeSettings>
{
    public void Configure(EntityTypeBuilder<SchemeSettings> e)
    {
        e.ToTable("SchemeSettings", Schema);
        e.HasKey(x => x.SchemeId);
        e.HasOne<Scheme>().WithOne().HasForeignKey<SchemeSettings>(x => x.SchemeId);
        Versioned(e);
        JsonList(e.Property(x => x.ContractTypes), 50);
        JsonList(e.Property(x => x.MurabahaRates), 200);
        JsonList(e.Property(x => x.RepaymentTermsMonths), 200);
        e.Property(x => x.QardHasanRate).HasPrecision(5, 2);
        e.Property(x => x.SalaryFromSepahMultiplierYes).HasPrecision(5, 2);
        e.Property(x => x.SalaryFromSepahMultiplierNo).HasPrecision(5, 2);
        e.HasMany(x => x.Plans).WithOne().HasForeignKey(p => p.SchemeId).HasPrincipalKey(s => s.SchemeId);
    }
}
