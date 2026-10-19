


namespace IdentityService.Domain.Entity;

public partial class UserOrganizationEntity
{
    public int Id { get; set; }

    public int OrganizationId { get; set; }

    public int UserId { get; set; }

    public virtual UserEntity User { get; set; } = null!;
}
