using IdentityService.Domain.Entity.UserManagement.UserManagementTokenLog;

namespace IdentityService.Infrastructure.Configuration;

public class UserManagmentLogEntityConfiguration : IEntityTypeConfiguration<UserManagementTokenLogEntity>
{
    public void Configure(EntityTypeBuilder<UserManagementTokenLogEntity> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_UserManagementTokenLog");

        builder.ToTable("UserManagementTokenLog");

        builder.Property(e => e.AccessToken)
            .HasMaxLength(500)
            .IsUnicode(false);
        builder.Property(e => e.CreateDate)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.DeviceInfo).HasMaxLength(500);
        builder.Property(e => e.ExpierDate).HasColumnType("datetime");
        builder.Property(e => e.Ip)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.RefreshToken)
            .HasMaxLength(500)
            .IsUnicode(false);
        builder.Property(e => e.UpdateDate).HasColumnType("datetime");

        builder.HasOne(d => d.User).WithMany(p => p.UserManagementTokenLogs)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserManagementTokenLog_User");
    }
}