using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Companies;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> e)
    {
        e.ToTable("Companies", Schema);
        Versioned(e);
        e.Property(x => x.BalancesVersion).IsConcurrencyToken();
        e.Property(x => x.Name).HasMaxLength(100).IsRequired();
        e.Property(x => x.NationalId).HasMaxLength(11).IsUnicode(false).IsRequired();
        e.Property(x => x.AgreementNumber).HasMaxLength(50).IsRequired();
        e.Property(x => x.AccountNumber).HasMaxLength(24).IsUnicode(false).IsRequired();
        e.Property(x => x.BranchCode).HasMaxLength(50).IsUnicode(false).IsRequired();
        e.Property(x => x.DeleteDescription).HasMaxLength(500);
        RegistrarColumn(e.Property(x => x.Registrar));
        e.HasOne<Scheme>().WithMany().HasForeignKey(x => x.SchemeId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne(x => x.AgreementFile).WithMany().HasForeignKey(x => x.AgreementFileId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne(x => x.Cancellation).WithOne().HasForeignKey<CompanyCancellation>(x => x.CompanyId);
        e.HasMany(x => x.Plans).WithOne().HasForeignKey(x => x.CompanyId);
        e.HasMany(x => x.Balances).WithOne().HasForeignKey(x => x.CompanyId);
        e.HasIndex(x => x.BranchCode);
        e.HasIndex(x => x.NationalId);
        e.Ignore(x => x.IsDeleted);
    }
}
