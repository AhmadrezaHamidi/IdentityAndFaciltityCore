namespace IdentityService.Infrastructure.Configuration;

public class ChangePasswordUserManagmentLogEntityConfiguration : IEntityTypeConfiguration<ChangePasswordUserManagmentLogEntity>
{
    public void Configure(EntityTypeBuilder<ChangePasswordUserManagmentLogEntity> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_ChangePasswordLog");

        builder.ToTable("ChangePasswordUserManagmentLog");

        builder.Property(e => e.CreateDate)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.DeviceInfo).HasMaxLength(500);
        builder.Property(e => e.Ip)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.LastPassword).HasMaxLength(500);
        builder.Property(e => e.NewPassword).HasMaxLength(500);

        builder.HasOne(d => d.User).WithMany(p => p.ChangePasswordUserManagmentLogs)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ChangePasswordLog_User");    }
}