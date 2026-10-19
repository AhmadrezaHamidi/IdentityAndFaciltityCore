namespace IdentityService.Infrastructure.Configuration;

public class UserOrganizationEntityConfiguration : IEntityTypeConfiguration<UserOrganizationEntity>
{
    public void Configure(EntityTypeBuilder<UserOrganizationEntity> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_OrganizationUsers");

        builder.ToTable("UserOrganization");

        builder.HasIndex(e => new { e.UserId, e.OrganizationId }, "UX_UserOrganization").IsUnique();

        builder.HasOne(d => d.User).WithMany(p => p.UserOrganizations)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserOrganization_User");
    }
}