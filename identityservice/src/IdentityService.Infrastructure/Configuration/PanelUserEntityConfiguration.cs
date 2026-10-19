namespace IdentityService.Infrastructure.Configuration;

public class PanelUserEntityConfiguration : IEntityTypeConfiguration<PanelUserEntity>
{
    public void Configure(EntityTypeBuilder<PanelUserEntity> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_BackOfficeUser");

        builder.ToTable("PanelUser");

        builder.HasIndex(e => e.IsActive, "IX_PanelUser_IsActive");

        builder.HasIndex(e => e.Email, "UK_PanelUser_Email")
            .IsUnique()
            .HasFilter("([IsActive]=(1))");

        builder.HasIndex(e => e.PersonId, "UX_PanelUser_PersonId").IsUnique();

        builder.HasIndex(e => e.UserName, "UX_PanelUser_UserName").IsUnique();

        builder.Property(e => e.CreationTime).HasColumnType("datetime");
        builder.Property(e => e.Email).HasMaxLength(100);
        builder.Property(e => e.PasswordHash)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.UserName).HasMaxLength(50);

        builder.HasOne(d => d.Person).WithOne(p => p.PanelUser)
            .HasForeignKey<PanelUserEntity>(d => d.PersonId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PanelUser_Person");
    }
}