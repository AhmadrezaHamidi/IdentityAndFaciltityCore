namespace IdentityService.Infrastructure.Configuration;

public class MerchantOmidBankEntityConfiguration : IEntityTypeConfiguration<MerchantOmidBankEntity>
{
    public void Configure(EntityTypeBuilder<MerchantOmidBankEntity> builder)
    {
        builder
            .HasNoKey()
            .ToTable("MerchantOmidBank");

        builder.Property(e => e.BranchsCount).HasMaxLength(50);
        builder.Property(e => e.MerchantName).HasMaxLength(100);
        builder.Property(e => e.MerchantType).HasMaxLength(50);
        builder.Property(e => e.SaleType).HasMaxLength(50);
    }
}