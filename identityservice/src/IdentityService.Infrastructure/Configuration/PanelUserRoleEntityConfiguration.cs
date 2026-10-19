namespace IdentityService.Infrastructure.Configuration;

public class PanelUserRoleEntityConfiguration : IEntityTypeConfiguration<PanelUserRoleEntity>
{
    public void Configure(EntityTypeBuilder<PanelUserRoleEntity> builder)
    {
        builder.ToTable("PanelUserRole");

        builder.HasIndex(e => new { e.PanelUserId, e.RoleId }, "UX_PanelUserRole_PanelUserId_RoleOd").IsUnique();

        builder.HasOne(d => d.PanelUser).WithMany(p => p.PanelUserRoles)
            .HasForeignKey(d => d.PanelUserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PanelUserRole_PanelUser");

        builder.HasOne(d => d.Role).WithMany(p => p.PanelUserRoles)
            .HasForeignKey(d => d.RoleId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PanelUserRole_Role");
        
    }
}