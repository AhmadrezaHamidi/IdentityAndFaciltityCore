using IdentityService.Domain.Entity.UserManagement.ChangeUserActiveStateHistory;

namespace IdentityService.Infrastructure.Configuration;

public class UserIsActiveChangeStateConfiguration : IEntityTypeConfiguration<ChangeUserActiveStateHistory>
{
    public void Configure(EntityTypeBuilder<ChangeUserActiveStateHistory> builder)
    {
        builder.ToTable("UserIsActiveChangeState");
    }
}
