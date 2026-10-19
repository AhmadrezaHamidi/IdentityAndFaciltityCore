namespace IdentityService.Domain.Entity;

/// <summary>
/// لاگ امنیتی تغییرات کاربر، نقش و دسترسی — دستهٔ «کاربران» در تب «گزارش تغییرات» سند.
///
/// جدا از <c>UserManagementSettingChangeHistory</c> است: آن یکی تغییرات تنظیمات مشتری را
/// ثبت می‌کند، این یکی تغییرات پنل ادمین را. سند برای گزارش تغییرات «نام کاربر و زمان و
/// جزئیات عملیات شامل مقدار قبلی و فعلی» را می‌خواهد، که BeforeJson/AfterJson همان است.
/// </summary>
public class PanelSecurityAuditLogEntity
{
    public long Id { get; set; }

    /// <summary>مثل role.update، panel-user.create، panel-user.active.change.</summary>
    public string Action { get; set; } = null!;

    /// <summary>کاربری که عمل را انجام داد. null برای عملیات سیستمی (مثل اجرای اسکریپت).</summary>
    public int? ActorPanelUserId { get; set; }

    /// <summary>Role، PanelUser، PanelMenu.</summary>
    public string TargetType { get; set; } = null!;

    public string? TargetId { get; set; }

    public string? BeforeJson { get; set; }

    public string? AfterJson { get; set; }

    public string? Ip { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }
}
