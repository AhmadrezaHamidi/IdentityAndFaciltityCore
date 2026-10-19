


namespace IdentityService.Domain.Entity;

public partial class RolePanelMenuEntity
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int PanelMenuId { get; set; }

    public virtual PanelMenuEntity PanelMenu { get; set; } = null!;

    public virtual RoleEntity Role { get; set; } = null!;
}
