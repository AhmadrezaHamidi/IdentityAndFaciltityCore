namespace IdentityService.Infrastructure.Configuration;

public class UserValidateLogEntityConfiguration : IEntityTypeConfiguration<UserValidateLogEntity>
{
    public void Configure(EntityTypeBuilder<UserValidateLogEntity> builder)
    {
        builder.ToTable("UserValidateLog");

        builder.Property(e => e.CreateDate).HasColumnType("datetime");
        builder.Property(e => e.InputData).HasMaxLength(1000);

        builder.HasOne(d => d.User).WithMany(p => p.UserValidateLogs)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserValidateLog_User");
    }
}