namespace IdentityService.Infrastructure.Configuration;

public class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Role");

        builder.HasIndex(e => e.PersianTitle, "UX_Role_PersianTitle").IsUnique();

        builder.HasIndex(e => e.Title, "UX_Role_Title").IsUnique();

        builder.Property(e => e.PersianTitle).HasMaxLength(100);
        builder.Property(e => e.Title)
            .HasMaxLength(100)
            .IsUnicode(false);
    }
}