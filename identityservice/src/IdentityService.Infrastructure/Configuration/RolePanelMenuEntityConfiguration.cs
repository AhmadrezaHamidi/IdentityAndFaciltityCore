namespace IdentityService.Infrastructure.Configuration;

public class RolePanelMenuEntityConfiguration : IEntityTypeConfiguration<RolePanelMenuEntity>
{
    public void Configure(EntityTypeBuilder<RolePanelMenuEntity> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_RoleMenu");

        builder.ToTable("RolePanelMenu");

        builder.HasOne(d => d.PanelMenu).WithMany(p => p.RolePanelMenus)
            .HasForeignKey(d => d.PanelMenuId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_RolePanelMenu_PanelMenu");

        builder.HasOne(d => d.Role).WithMany(p => p.RolePanelMenus)
            .HasForeignKey(d => d.RoleId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_RolePanelMenu_Role");    }
}