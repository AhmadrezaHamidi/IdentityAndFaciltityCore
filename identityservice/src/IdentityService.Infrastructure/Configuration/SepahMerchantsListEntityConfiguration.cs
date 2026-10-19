namespace IdentityService.Infrastructure.Configuration;

public class SepahMerchantsListEntityConfiguration : IEntityTypeConfiguration<SepahMerchantsListEntity>
{
    public void Configure(EntityTypeBuilder<SepahMerchantsListEntity> builder)
    {
        builder
            .HasNoKey()
            .ToTable("SepahMerchantsList");

        builder.Property(e => e.BranchsCount).HasMaxLength(50);
        builder.Property(e => e.MerchantName).HasMaxLength(100);
        builder.Property(e => e.SaleType).HasMaxLength(50);
    }
}