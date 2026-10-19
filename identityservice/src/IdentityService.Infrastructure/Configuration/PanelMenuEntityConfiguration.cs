namespace IdentityService.Infrastructure.Configuration;

public class PanelMenuEntityConfiguration : IEntityTypeConfiguration<PanelMenuEntity>
{
    public void Configure(EntityTypeBuilder<PanelMenuEntity> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_AdminPanelMenu");

        builder.ToTable("PanelMenu");

        builder.HasIndex(e => e.IsActive, "IX_PanelMenu_IsActive");

        builder.HasIndex(e => e.ParentId, "IX_PanelMenu_ParentId");

        builder.Property(e => e.ActionName)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.ControllerName)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.CreationTime).HasColumnType("datetime");
        builder.Property(e => e.EnTitle)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.IconName)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.Title).HasMaxLength(100);
        builder.Property(e => e.Url)
            .HasMaxLength(200)
            .IsUnicode(false);

        builder.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
            .HasForeignKey(d => d.ParentId)
            .HasConstraintName("FK_PanelMenu_PanelMenu");
    }
}