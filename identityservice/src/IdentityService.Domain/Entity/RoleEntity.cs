namespace IdentityService.Domain.Entity;

public partial class RoleEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string PersianTitle { get; set; } = null!;

    public bool IsActive { get; set; }

    /// <summary>توضیحات نقش — طبق طرح پنل مدیریت مجوزها (Figma). قبلاً روی این جدول نبود.</summary>
    public string? Description { get; set; }

    /// <summary>تاریخ ایجاد — طبق طرح پنل مدیریت مجوزها. رکوردهای قدیمی مقدار NULL دارند.</summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>شناسهٔ کاربر پنلی که این نقش را ایجاد کرده — طبق طرح پنل مدیریت مجوزها.</summary>
    public int? CreatedByPanelUserId { get; set; }

    /// <summary>
    /// نقش (0) یا گروه (1).
    ///
    /// در طرح Figma تب «مدیریت مجوزها» و تب «مدیریت گروه‌ها» دقیقاً یک فرم دارند
    /// (نام، کاربران، دسترسی‌ها، توضیحات، وضعیت) و هر دو همان نگاشت
    /// «صاحب دسترسی ← دسترسی‌ها» هستند. پس به‌جای سه جدول جدید، همین جدول با یک
    /// ستون تفکیک‌کننده هر دو را پوشش می‌دهد و جدول‌های واسط موجود
    /// (PanelUserRole و RolePanelMenu) بدون تغییر برای هر دو کار می‌کنند.
    /// یکتایی نام هم به‌ازای هر Kind جداگانه بررسی می‌شود.
    /// </summary>
    public int Kind { get; set; }

    public virtual ICollection<PanelUserRoleEntity> PanelUserRoles { get; set; } = new List<PanelUserRoleEntity>();

    public virtual ICollection<RolePanelMenuEntity> RolePanelMenus { get; set; } = new List<RolePanelMenuEntity>();
}
