using IdentityService.Domain.Entity.Basic.BankDataCity;

namespace IdentityService.Infrastructure.Configuration.Basic;

public class BankDataCityEntityConfiguration : IEntityTypeConfiguration<BankDataCityEntity>
{
    public void Configure(EntityTypeBuilder<BankDataCityEntity> builder)
    {
        builder.ToTable("BankDataCity","Basic");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Title).IsUnicode().IsRequired();
        builder.Property(e => e.BankId).HasMaxLength(30).IsRequired();
        builder.Property(e => e.BankStateId).HasMaxLength(30).IsRequired();
    }
}