namespace IdentityService.Infrastructure.Configuration;

public class FileEntityConfiguration : IEntityTypeConfiguration<FileEntity>
{
    public void Configure(EntityTypeBuilder<FileEntity> builder)
    {
        builder.ToTable("File");

        builder.HasIndex(e => e.FileTypeId, "IX_File_FileTypeId");

        builder.HasIndex(e => e.IsActive, "IX_File_IsActive");

        builder.HasIndex(e => e.IsDeleted, "IX_File_IsDeleted");

        builder.HasIndex(e => e.NameWithExtension, "IX_File_NameWithExtension");

        builder.HasIndex(e => e.ParentId, "IX_File_ParentId");

        builder.Property(e => e.BaseFileName).HasMaxLength(350);
        builder.Property(e => e.DisplaySize).HasMaxLength(50);
        builder.Property(e => e.EntityName).HasMaxLength(50);
        builder.Property(e => e.Extension).HasMaxLength(50);
        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValueSql("((1))");
        builder.Property(e => e.MimeType).HasMaxLength(50);
        builder.Property(e => e.MobileNumber).HasMaxLength(15);
        builder.Property(e => e.Name).HasMaxLength(350);
        builder.Property(e => e.NameWithExtension).HasMaxLength(350);
        builder.Property(e => e.RegisterDate)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Title).HasMaxLength(250);
        builder.Property(e => e.UploadFileName).HasMaxLength(350);
        builder.Property(e => e.Url).HasMaxLength(500);

        builder.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
            .HasForeignKey(d => d.ParentId)
            .HasConstraintName("FK_File_File");
    }
}