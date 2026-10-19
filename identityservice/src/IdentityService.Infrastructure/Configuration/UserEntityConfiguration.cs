namespace IdentityService.Infrastructure.Configuration;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("User");

        builder.Property(e => e.FirebaseKey).HasMaxLength(1000);
        builder.Property(e => e.InvitedCode)
            .HasMaxLength(11)
            .IsUnicode(false);

        builder.Property(e => e.BankCustomerNumber).IsRequired(false);
        builder.Property(e => e.MyInviteCode)
            .HasMaxLength(11)
            .IsUnicode(false);

        builder.Property(e => e.IsActive).HasDefaultValue(true);

        builder.HasOne(d => d.Person).WithMany(p => p.Users)
            .HasForeignKey(d => d.PersonId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_User_Person");
    }
}