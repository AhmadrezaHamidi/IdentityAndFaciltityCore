using IdentityService.Domain.Entity.UserManagement.LoginLogOutHistory;

namespace IdentityService.Infrastructure.Configuration;

public class LoginLogoutHistoryConfiguration : IEntityTypeConfiguration<LoginLogoutHistoryEntity>
{
    public void Configure(EntityTypeBuilder<LoginLogoutHistoryEntity> builder)
    {
        builder.ToTable("LoginLogoutHistory");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TypeAction).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
        builder.Property(e => e.DeviceInfo).HasMaxLength(500);
        builder.Property(e => e.Ip).HasMaxLength(50).IsUnicode(false);

        builder.HasOne(d => d.User).WithMany(p => p.LoginLogoutHistories)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_LoginLogoutHistory_User");
    }
}