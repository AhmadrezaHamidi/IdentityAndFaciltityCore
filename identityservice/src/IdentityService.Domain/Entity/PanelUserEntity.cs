


namespace IdentityService.Domain.Entity;

public partial class PanelUserEntity
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreationTime { get; set; }

    public virtual ICollection<PanelUserRoleEntity> PanelUserRoles { get; set; } = new List<PanelUserRoleEntity>();

    public virtual PersonEntity Person { get; set; } = null!;
}
