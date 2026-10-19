using IdentityService.Domain.Entity.UserManagement.UserManagmentBiometric;

namespace IdentityService.Infrastructure.Configuration;

public class UserManagmentBiometricEntityConfiguration : IEntityTypeConfiguration<UserManagmentBiometricEntity>
{
    public void Configure(EntityTypeBuilder<UserManagmentBiometricEntity> builder)
    {
        builder.ToTable("UserManagmentBiometrics");

        builder.Property(e => e.BiometricKey).HasMaxLength(50);
        builder.Property(e => e.BiometricToken).HasMaxLength(500);
        builder.Property(e => e.CreateDate)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
    }
}