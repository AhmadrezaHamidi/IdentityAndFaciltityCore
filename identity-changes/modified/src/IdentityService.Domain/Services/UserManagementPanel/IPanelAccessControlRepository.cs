using IdentityService.Domain.Authorization;
using IdentityService.Domain.Entity;

namespace IdentityService.Domain.Services.UserManagementPanel;

/// <summary>
/// دسترسی مستقیم EF برای تب‌های «مدیریت کاربران» پنل ادمین
/// (مدیریت دسترسی‌ها / مجوزها / گروه‌ها / کاربران).
///
/// عمداً از <c>IRoleRepository</c> و <c>IPanelMenuRepository</c> موجود استفاده
/// نمی‌کند: آن‌ها Dapper + Stored Procedure هستند و فیلدهای جدید طرح (توضیحات،
/// تاریخ ایجاد، ایجادکننده، تفکیک نقش/گروه) در SPهایشان وجود ندارد. طبق تصمیم
/// «بدون SP جدید، مستقیم با EF»، این رپازیتوری همان جدول‌ها را با EF می‌خواند و
/// می‌نویسد. رپازیتوری‌های قدیمی دست‌نخورده باقی می‌مانند تا جریان‌های موجود نشکنند.
/// </summary>
public interface IPanelAccessControlRepository
{
    // ── تب ۱: مدیریت دسترسی‌ها (PanelMenu) ────────────────────────────────────
    Task<(List<PanelMenuEntity> Items, int Total)> SearchAccessesAsync(
        string? term, int page, int pageSize, CancellationToken cancellationToken);

    Task<PanelMenuEntity?> GetAccessAsync(int id, CancellationToken cancellationToken);

    Task<bool> AccessTitleExistsAsync(string title, int? exceptId, CancellationToken cancellationToken);

    Task<int> AddAccessAsync(PanelMenuEntity access, CancellationToken cancellationToken);

    Task<bool> UpdateAccessAsync(PanelMenuEntity access, CancellationToken cancellationToken);

    // ── تب ۲ و ۳: مجوزها (Kind=0) و گروه‌ها (Kind=1) — هر دو روی جدول Role ─────
    Task<(List<RoleEntity> Items, int Total)> SearchRolesAsync(
        int kind, string? term, int page, int pageSize, CancellationToken cancellationToken);

    Task<RoleEntity?> GetRoleAsync(int id, int kind, CancellationToken cancellationToken);

    Task<bool> RoleTitleExistsAsync(int kind, string title, int? exceptId, CancellationToken cancellationToken);

    Task<int> AddRoleAsync(RoleEntity role, List<int> menuIds, List<int> panelUserIds, CancellationToken cancellationToken);

    Task<bool> UpdateRoleAsync(RoleEntity role, List<int> menuIds, List<int> panelUserIds, CancellationToken cancellationToken);

    Task<List<int>> GetRoleMenuIdsAsync(int roleId, CancellationToken cancellationToken);

    Task<List<PanelUserEntity>> GetRoleUsersAsync(int roleId, CancellationToken cancellationToken);

    /// <summary>نگاشت نام کاربری → شناسه؛ برای فرم «افزودن کاربر/کاربران با پارامتر username».</summary>
    Task<List<PanelUserEntity>> GetPanelUsersByUserNamesAsync(List<string> userNames, CancellationToken cancellationToken);

    // ── تب ۴: مدیریت کاربران پنل ─────────────────────────────────────────────
    Task<(List<PanelUserEntity> Items, int Total)> SearchPanelUsersAsync(
        string? term, int page, int pageSize, CancellationToken cancellationToken);

    Task<PanelUserEntity?> GetPanelUserAsync(int id, CancellationToken cancellationToken);

    Task<bool> SetPanelUserActiveAsync(int id, bool isActive, CancellationToken cancellationToken);

    // ── مجوزهای مؤثر و ابطال نشست ─────────────────────────────────────────────

    /// <summary>کاربر با Includeهای لازم برای EffectivePermissions.Build.</summary>
    Task<PanelUserEntity?> GetPanelUserWithPermissionsAsync(int panelUserId, CancellationToken cancellationToken);

    /// <summary>معیار پذیرش ۶ — نسخهٔ مجوز را بالا می‌برد و کش نشست را دور می‌اندازد.</summary>
    Task BumpPermissionVersionAsync(IReadOnlyCollection<int> panelUserIds, CancellationToken cancellationToken);

    Task BumpPermissionVersionForRoleAsync(int roleId, CancellationToken cancellationToken);

    // ── لاگ امنیتی ────────────────────────────────────────────────────────────

    Task WriteAuditAsync(PanelSecurityAuditLogEntity entry, CancellationToken cancellationToken);

    Task<(List<PanelSecurityAuditLogEntity> Items, int Total)> SearchAuditAsync(
        string? action, string? targetType, int page, int pageSize, CancellationToken cancellationToken);

    // ── ساخت و ویرایش کاربر پنل ───────────────────────────────────────────────

    Task<bool> PanelUserNameExistsAsync(string userName, int? exceptId, CancellationToken cancellationToken);

    Task<bool> PanelUserCodeExistsAsync(string userCode, int? exceptId, CancellationToken cancellationToken);

    Task<int> AddPanelUserAsync(
        PanelUserEntity user, IReadOnlyCollection<RoleAssignment> assignments, CancellationToken cancellationToken);

    /// <param name="assignments">null یعنی نقش‌ها دست نخورند؛ فهرست خالی یعنی همه برداشته شوند.</param>
    Task<bool> UpdatePanelUserAsync(
        PanelUserEntity user, IReadOnlyCollection<RoleAssignment>? assignments, CancellationToken cancellationToken);
}

/// <summary>یک نقش با دامنه‌اش، همان‌طور که فرم «مدیریت کاربران» می‌فرستد.</summary>
public sealed record RoleAssignment(
    int RoleId,
    AccessScopeType ScopeType,
    string? ScopeId,
    int? CreatedByPanelUserId = null);
