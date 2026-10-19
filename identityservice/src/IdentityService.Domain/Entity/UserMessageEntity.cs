


namespace IdentityService.Domain.Entity;

public partial class UserMessageEntity
{
    public long Id { get; set; }

    public int MessageId { get; set; }

    public int UserId { get; set; }

    public virtual UserEntity User { get; set; } = null!;
}
