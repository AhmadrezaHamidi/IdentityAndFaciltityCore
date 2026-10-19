namespace IdentityService.Infrastructure.Configuration;

public class UserManagmentEntityConfiguration : IEntityTypeConfiguration<UserManagmentEntity>
{
    public void Configure(EntityTypeBuilder<UserManagmentEntity> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_Table_1");

        builder.ToTable("UserManagment");


        builder.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");


        builder.Property(e => e.PasswordUpdateDate).HasColumnType("datetime");
        builder.Property(e => e.TakeCertificateDate).HasColumnType("datetime");
        builder.Property(e => e.TosanRegister).IsRequired(false);
        
        
        builder.Property(e => e.AutoUpdateCif).IsRequired(false);
        builder.Property(e => e.TryCountAutoUpdateCif).IsRequired(false);
        builder.Property(e => e.AdditionalDescription).IsRequired(false);

        builder.HasOne(d => d.User).WithMany(p => p.UserManagments)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Table_1_User");
    }
}