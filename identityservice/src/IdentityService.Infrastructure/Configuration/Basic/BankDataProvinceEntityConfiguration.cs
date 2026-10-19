using IdentityService.Domain.Entity.Basic.BankDataProvince;

namespace IdentityService.Infrastructure.Configuration.Basic;

public class BankDataProvinceEntityConfiguration : IEntityTypeConfiguration<BankDataProvinceEntity>
{
    public void Configure(EntityTypeBuilder<BankDataProvinceEntity> builder)
    {     
        builder.ToTable("BankDataProvince","Basic");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Title).IsUnicode().IsRequired();
        builder.Property(e => e.BankId).HasMaxLength(30).IsRequired();
    }
}