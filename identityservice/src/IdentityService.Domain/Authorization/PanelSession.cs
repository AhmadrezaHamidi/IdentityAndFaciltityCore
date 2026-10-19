namespace IdentityService.Domain.Authorization;

/// <summary>
/// کاربر پنل جاری با مجوزهای لحظه‌ای.
/// در لایهٔ Domain است تا هم فیلتر Admin.Api و هم پیاده‌سازی Infrastructure به آن دسترسی داشته باشند.
/// </summary>
public sealed record PanelSession(
    int PanelUserId,
    string UserName,
    string? FullName,
    string? UserCode,
    string? BranchCode,
    string? BranchName,
    EffectivePermissions Permissions);

/// <summary>
/// توکن ← کاربر جاری با مجوزهای لحظه‌ای. null یعنی نشست باطل است:
/// توکن نامعتبر، کاربر غیرفعال، یا دسترسی عوض شده (معیار پذیرش ۶).
/// </summary>
public interface IPanelSessionResolver
{
    Task<PanelSession?> ResolveAsync(CancellationToken cancellationToken);

    /// <summary>کش نشست یک کاربر را دور می‌اندازد — بعد از تغییر دسترسی یا خروج اجباری.</summary>
    void Invalidate(int panelUserId);
}
