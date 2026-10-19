namespace IdentityService.Infrastructure.Configuration;

public class UserMessageEntityConfiguration : IEntityTypeConfiguration<UserMessageEntity>
{
    public void Configure(EntityTypeBuilder<UserMessageEntity> builder)
    {
        builder.ToTable("UserMessage");

        builder.HasOne(d => d.User).WithMany(p => p.UserMessages)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserMessage_User");
    }
}