namespace IdentityService.Infrastructure.Configuration;

public class UserManagementKycLogEntityConfiguration : IEntityTypeConfiguration<UserManagementKycLogEntity>
{
    public void Configure(EntityTypeBuilder<UserManagementKycLogEntity> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_UserManagementKyc");

        builder.ToTable("UserManagementKycLog");


        builder.Property(e => e.NationalCardSeries).HasMaxLength(10);
        builder.Property(e => e.TokenExpierTime).HasColumnType("datetime").IsRequired(false);
        builder.Property(e => e.BirthDate).HasColumnType("datetime").IsRequired(false);
        builder.Property(e => e.Token).IsRequired(false);


        builder.HasOne(d => d.User).WithMany(p => p.UserManagementKycLogs)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("FK_UserManagementKycLog_User");
    }
}