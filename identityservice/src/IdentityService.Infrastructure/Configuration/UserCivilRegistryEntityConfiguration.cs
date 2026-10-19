namespace IdentityService.Infrastructure.Configuration;

public class UserCivilRegistryEntityConfiguration : IEntityTypeConfiguration<UserCivilRegistryEntity>
{
    public void Configure(EntityTypeBuilder<UserCivilRegistryEntity> builder)
    {
        builder.ToTable("UserCivilRegistry");

        builder.Property(e => e.BirthDate)
            .HasMaxLength(8)
            .IsUnicode(false);
        builder.Property(e => e.CardSerial)
            .HasMaxLength(12)
            .IsUnicode(false);
        builder.Property(e => e.Family).HasMaxLength(50);
        builder.Property(e => e.FatherName).HasMaxLength(50);
        builder.Property(e => e.Name).HasMaxLength(50);
        builder.Property(e => e.NationalId)
            .HasMaxLength(11)
            .IsUnicode(false);
        builder.Property(e => e.OfficeName).HasMaxLength(500);
        builder.Property(e => e.ShenasnameSeri)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.ShenasnameSerial)
            .HasMaxLength(6)
            .IsUnicode(false);
        builder.Property(e => e.Zipcode)
            .HasMaxLength(10)
            .IsUnicode(false);
        builder.Property(e => e.ZipcodeDesc)
            .HasMaxLength(1000)
            .HasColumnName("Zipcode_Desc");

        builder.HasOne(d => d.User).WithMany(p => p.UserCivilRegistries)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserCivilRegistry_User");
    }
}