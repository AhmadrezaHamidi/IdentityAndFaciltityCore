


namespace IdentityService.Domain.Entity;

public partial class PanelMenuEntity
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public string Title { get; set; } = null!;

    public string? EnTitle { get; set; }

    public string? Url { get; set; }

    public string? IconName { get; set; }

    public string? ActionName { get; set; }

    public string? ControllerName { get; set; }

    public bool IsRootMenu { get; set; }

    public bool IsInternalMenu { get; set; }

    public DateTime CreationTime { get; set; }

    public bool IsActive { get; set; }

    /// <summary>توضیحات دسترسی — طبق طرح تب «مدیریت دسترسی‌ها» (Figma). قبلاً روی این جدول نبود.</summary>
    public string? Description { get; set; }

    public virtual ICollection<PanelMenuEntity> InverseParent { get; set; } = new List<PanelMenuEntity>();

    public virtual PanelMenuEntity? Parent { get; set; }

    public virtual ICollection<RolePanelMenuEntity> RolePanelMenus { get; set; } = new List<RolePanelMenuEntity>();
}
