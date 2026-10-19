


namespace IdentityService.Domain.Entity;

public partial class PersonOpenAccountEntity
{
    public long Id { get; set; }

    public int PersonId { get; set; }

    public Guid UniqueId { get; set; }

    public DateTime ExpiryTime { get; set; }

    public virtual PersonEntity Person { get; set; } = null!;
}
