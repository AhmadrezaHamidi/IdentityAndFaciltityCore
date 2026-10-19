namespace IdentityService.Infrastructure.Configuration;

public class UserBranchEntityConfiguration : IEntityTypeConfiguration<UserBranchEntity>
{
    public void Configure(EntityTypeBuilder<UserBranchEntity> builder)
    {
        builder.Property(e => e.BranchCode)
            .HasMaxLength(15)
            .IsUnicode(false);
        builder.Property(e => e.CreationTime)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
    }
}