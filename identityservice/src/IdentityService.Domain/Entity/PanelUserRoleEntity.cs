


namespace IdentityService.Domain.Entity;

public partial class PanelUserRoleEntity
{
    public int Id { get; set; }

    public int PanelUserId { get; set; }

    public int RoleId { get; set; }

    public virtual PanelUserEntity PanelUser { get; set; } = null!;

    public virtual RoleEntity Role { get; set; } = null!;
}
