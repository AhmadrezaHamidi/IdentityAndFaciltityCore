using IdentityService.Domain.Entity.Basic.BankData;

namespace IdentityService.Infrastructure.Configuration.Basic;

public class BankDataEntityConfiguration : IEntityTypeConfiguration<BankDataEntity>
{
    public void Configure(EntityTypeBuilder<BankDataEntity> builder)
    {
        builder.ToTable("BankData","Basic");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Title).IsUnicode().IsRequired();
        builder.Property(e => e.CategoryTypeId).IsRequired();
        builder.Property(e => e.BankId).HasMaxLength(30).IsRequired();
    }
}