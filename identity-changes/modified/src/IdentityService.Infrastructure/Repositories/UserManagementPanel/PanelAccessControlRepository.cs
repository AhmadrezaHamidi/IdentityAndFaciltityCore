using IdentityService.Domain.Authorization;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Services.UserManagementPanel;
using IdentityService.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories.UserManagementPanel;

public sealed class PanelAccessControlRepository : IPanelAccessControlRepository
{
    private readonly ApplicationDBContext _db;
    private readonly IPanelSessionResolver? _sessions;

    /// <param name="sessions">
    /// اختیاری تا این رپازیتوری در تست‌ها و اسکریپت‌ها بدون چرخهٔ وابستگی ساخته شود.
    /// وقتی هست، تغییر دسترسی کش نشست را فوراً دور می‌اندازد.
    /// </param>
    public PanelAccessControlRepository(ApplicationDBContext db, IPanelSessionResolver? sessions = null)
    {
        _db = db;
        _sessions = sessions;
    }

    private static int NormalizePage(int page) => page <= 0 ? 1 : page;
    private static int NormalizePageSize(int pageSize) => pageSize is <= 0 or > 200 ? 20 : pageSize;

    // ── تب ۱: مدیریت دسترسی‌ها ────────────────────────────────────────────────
    public async Task<(List<PanelMenuEntity> Items, int Total)> SearchAccessesAsync(
        string? term, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _db.PanelMenu.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(term))
            query = query.Where(m => m.Title.Contains(term));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(m => m.Id)
            .Skip((NormalizePage(page) - 1) * NormalizePageSize(pageSize))
            .Take(NormalizePageSize(pageSize))
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<PanelMenuEntity?> GetAccessAsync(int id, CancellationToken cancellationToken)
        => _db.PanelMenu.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public Task<bool> AccessTitleExistsAsync(string title, int? exceptId, CancellationToken cancellationToken)
        => _db.PanelMenu.AnyAsync(m => m.Title == title && (exceptId == null || m.Id != exceptId), cancellationToken);

    public async Task<int> AddAccessAsync(PanelMenuEntity access, CancellationToken cancellationToken)
    {
        await _db.PanelMenu.AddAsync(access, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return access.Id;
    }

    public async Task<bool> UpdateAccessAsync(PanelMenuEntity access, CancellationToken cancellationToken)
    {
        var current = await _db.PanelMenu.FirstOrDefaultAsync(m => m.Id == access.Id, cancellationToken);
        if (current is null) return false;

        current.Title = access.Title;
        current.EnTitle = access.EnTitle;
        current.Url = access.Url;
        current.IconName = access.IconName;
        current.ParentId = access.ParentId;
        current.Description = access.Description;
        current.IsActive = access.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    // ── تب ۲ و ۳: مجوزها (Kind=0) و گروه‌ها (Kind=1) ─────────────────────────
    public async Task<(List<RoleEntity> Items, int Total)> SearchRolesAsync(
        int kind, string? term, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _db.Role.AsNoTracking().Where(r => r.Kind == kind);

        if (!string.IsNullOrWhiteSpace(term))
            query = query.Where(r => r.PersianTitle.Contains(term) || r.Title.Contains(term));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.Id)
            .Skip((NormalizePage(page) - 1) * NormalizePageSize(pageSize))
            .Take(NormalizePageSize(pageSize))
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<RoleEntity?> GetRoleAsync(int id, int kind, CancellationToken cancellationToken)
        => _db.Role.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && r.Kind == kind, cancellationToken);

    public Task<bool> RoleTitleExistsAsync(int kind, string title, int? exceptId, CancellationToken cancellationToken)
        => _db.Role.AnyAsync(
            r => r.Kind == kind && r.PersianTitle == title && (exceptId == null || r.Id != exceptId),
            cancellationToken);

    public async Task<int> AddRoleAsync(RoleEntity role, List<int> menuIds, List<int> panelUserIds, CancellationToken cancellationToken)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        await _db.Role.AddAsync(role, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        await ReplaceRoleLinksAsync(role.Id, menuIds, panelUserIds, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return role.Id;
    }

    public async Task<bool> UpdateRoleAsync(RoleEntity role, List<int> menuIds, List<int> panelUserIds, CancellationToken cancellationToken)
    {
        var current = await _db.Role.FirstOrDefaultAsync(r => r.Id == role.Id && r.Kind == role.Kind, cancellationToken);
        if (current is null) return false;

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        current.PersianTitle = role.PersianTitle;
        current.Title = role.Title;
        current.Description = role.Description;
        current.IsActive = role.IsActive;
        await _db.SaveChangesAsync(cancellationToken);

        await ReplaceRoleLinksAsync(current.Id, menuIds, panelUserIds, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// جایگزینی کامل دسترسی‌ها و کاربرانِ یک نقش/گروه.
    /// فرم طرح، فهرست کامل را می‌فرستد نه تغییرات جزئی — پس حذف‌وثبت دوباره،
    /// هم ساده‌تر است هم با آنچه کاربر در صفحه می‌بیند یکی می‌ماند.
    /// </summary>
    private async Task ReplaceRoleLinksAsync(int roleId, List<int> menuIds, List<int> panelUserIds, CancellationToken cancellationToken)
    {
        var currentMenus = await _db.RolePanelMenu.Where(x => x.RoleId == roleId).ToListAsync(cancellationToken);
        _db.RolePanelMenu.RemoveRange(currentMenus);

        var currentUsers = await _db.PanelUserRole.Where(x => x.RoleId == roleId).ToListAsync(cancellationToken);
        var previousScopes = currentUsers
            .GroupBy(x => x.PanelUserId)
            .ToDictionary(g => g.Key, g => g.First());
        _db.PanelUserRole.RemoveRange(currentUsers);

        await _db.SaveChangesAsync(cancellationToken);

        if (menuIds.Count > 0)
        {
            var validMenuIds = await _db.PanelMenu
                .Where(m => menuIds.Contains(m.Id))
                .Select(m => m.Id)
                .ToListAsync(cancellationToken);

            await _db.RolePanelMenu.AddRangeAsync(
                validMenuIds.Distinct().Select(id => new RolePanelMenuEntity { RoleId = roleId, PanelMenuId = id }),
                cancellationToken);
        }

        if (panelUserIds.Count > 0)
        {
            var validUsers = await _db.PanelUser
                .Where(u => panelUserIds.Contains(u.Id))
                .Select(u => new { u.Id, u.BranchCode })
                .ToListAsync(cancellationToken);

            // دامنه روی انتساب است، و این متد انتساب‌ها را پاک و بازسازی می‌کند. بدون این،
            // هر ویرایش نقش از تب مجوزها دامنهٔ همهٔ کاربرانش را صفر می‌کرد و کاربر شعبه
            // یک‌شبه سراسری می‌شد. پس دامنهٔ قبلی را برمی‌گردانیم و برای انتساب جدید،
            // پیش‌فرض را از کد شعبهٔ خود کاربر برمی‌داریم.
            await _db.PanelUserRole.AddRangeAsync(
                validUsers.DistinctBy(u => u.Id).Select(u =>
                {
                    var previous = previousScopes.GetValueOrDefault(u.Id);
                    var scopeType = previous?.ScopeType
                        ?? (string.IsNullOrWhiteSpace(u.BranchCode) ? AccessScopeType.Global : AccessScopeType.Branch);
                    var scopeId = previous is not null ? previous.ScopeId : u.BranchCode;

                    return new PanelUserRoleEntity
                    {
                        RoleId = roleId,
                        PanelUserId = u.Id,
                        ScopeType = scopeType,
                        ScopeId = scopeType == AccessScopeType.Global ? null : scopeId,
                        CreatedAt = DateTime.UtcNow,
                    };
                }),
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);

        // معیار پذیرش ۶: دسترسی که عوض شد، باید کاربر را از سامانه بیندازد بیرون.
        // این‌جا تنها جایی است که مجوزهای یک نقش یا فهرست کاربرانش تغییر می‌کند، پس
        // bump همین‌جا است و نه در Handlerها — وگرنه یک Handler جدید می‌تواند فراموشش کند.
        // هم کاربران قبلی (که نقش را از دست دادند) و هم کاربران جدید باید دوباره وارد شوند.
        var affected = previousScopes.Keys.Concat(panelUserIds).Distinct().ToList();
        await BumpPermissionVersionAsync(affected, cancellationToken);
    }

    public Task<List<int>> GetRoleMenuIdsAsync(int roleId, CancellationToken cancellationToken)
        => _db.RolePanelMenu.AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .Select(x => x.PanelMenuId)
            .ToListAsync(cancellationToken);

    public Task<List<PanelUserEntity>> GetRoleUsersAsync(int roleId, CancellationToken cancellationToken)
        => _db.PanelUserRole.AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .Select(x => x.PanelUser)
            .ToListAsync(cancellationToken);

    public Task<List<PanelUserEntity>> GetPanelUsersByUserNamesAsync(List<string> userNames, CancellationToken cancellationToken)
        => _db.PanelUser.AsNoTracking()
            .Where(u => userNames.Contains(u.UserName))
            .ToListAsync(cancellationToken);

    // ── تب ۴: کاربران پنل ────────────────────────────────────────────────────
    public async Task<(List<PanelUserEntity> Items, int Total)> SearchPanelUsersAsync(
        string? term, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _db.PanelUser.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(term))
            query = query.Where(u => u.UserName.Contains(term) || u.Email.Contains(term));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(u => u.Id)
            .Skip((NormalizePage(page) - 1) * NormalizePageSize(pageSize))
            .Take(NormalizePageSize(pageSize))
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<PanelUserEntity?> GetPanelUserAsync(int id, CancellationToken cancellationToken)
        => _db.PanelUser.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<bool> SetPanelUserActiveAsync(int id, bool isActive, CancellationToken cancellationToken)
    {
        var user = await _db.PanelUser.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null) return false;

        user.IsActive = isActive;
        await _db.SaveChangesAsync(cancellationToken);

        // غیرفعال‌شدن هم تغییر دسترسی است: کاربر باید همین حالا بیرون بیفتد، نه در ورود بعدی.
        await BumpPermissionVersionAsync([id], cancellationToken);
        return true;
    }

    // ── مجوزهای مؤثر و ابطال نشست ─────────────────────────────────────────────

    /// <summary>
    /// کاربر با هر چه EffectivePermissions لازم دارد. AsNoTracking چون فقط خوانده می‌شود
    /// و AsSplitQuery چون سه سطح Include روی یک کوئری، سطرها را در هم ضرب می‌کند.
    /// </summary>
    public Task<PanelUserEntity?> GetPanelUserWithPermissionsAsync(int panelUserId, CancellationToken cancellationToken)
        => _db.PanelUser
            .AsNoTracking()
            .AsSplitQuery()
            .Include(u => u.PanelUserRoles)
                .ThenInclude(a => a.Role)
                    .ThenInclude(r => r.RolePanelMenus)
                        .ThenInclude(rm => rm.PanelMenu)
            .FirstOrDefaultAsync(u => u.Id == panelUserId, cancellationToken);

    /// <summary>
    /// معیار پذیرش ۶. نسخه را بالا می‌برد تا توکن‌های صادرشده با نسخهٔ قدیمی باطل شوند،
    /// و کش نشست همان کاربران را هم دور می‌اندازد تا تأخیر صفر شود.
    /// </summary>
    public async Task BumpPermissionVersionAsync(IReadOnlyCollection<int> panelUserIds, CancellationToken cancellationToken)
    {
        if (panelUserIds.Count == 0) return;

        var users = await _db.PanelUser
            .Where(u => panelUserIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        foreach (var user in users)
        {
            user.PermissionVersion += 1;
            user.LastPermissionChangeAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);

        foreach (var user in users)
            _sessions?.Invalidate(user.Id);
    }

    public async Task BumpPermissionVersionForRoleAsync(int roleId, CancellationToken cancellationToken)
    {
        var ids = await _db.PanelUserRole
            .Where(x => x.RoleId == roleId)
            .Select(x => x.PanelUserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        await BumpPermissionVersionAsync(ids, cancellationToken);
    }

    // ── لاگ امنیتی ────────────────────────────────────────────────────────────

    public async Task WriteAuditAsync(PanelSecurityAuditLogEntity entry, CancellationToken cancellationToken)
    {
        entry.CreatedAt = DateTime.UtcNow;
        await _db.PanelSecurityAuditLog.AddAsync(entry, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<(List<PanelSecurityAuditLogEntity> Items, int Total)> SearchAuditAsync(
        string? action, string? targetType, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _db.PanelSecurityAuditLog.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(action)) query = query.Where(x => x.Action == action);
        if (!string.IsNullOrWhiteSpace(targetType)) query = query.Where(x => x.TargetType == targetType);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((NormalizePage(page) - 1) * NormalizePageSize(pageSize))
            .Take(NormalizePageSize(pageSize))
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    // ── ساخت و ویرایش کاربر پنل ───────────────────────────────────────────────

    public Task<bool> PanelUserNameExistsAsync(string userName, int? exceptId, CancellationToken cancellationToken)
        => _db.PanelUser.AnyAsync(u => u.UserName == userName && (exceptId == null || u.Id != exceptId), cancellationToken);

    public Task<bool> PanelUserCodeExistsAsync(string userCode, int? exceptId, CancellationToken cancellationToken)
        => _db.PanelUser.AnyAsync(u => u.UserCode == userCode && (exceptId == null || u.Id != exceptId), cancellationToken);

    public async Task<int> AddPanelUserAsync(
        PanelUserEntity user,
        IReadOnlyCollection<RoleAssignment> assignments,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        user.CreationTime = DateTime.UtcNow;
        user.PermissionVersion = 1;
        await _db.PanelUser.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        await ReplaceUserAssignmentsAsync(user.Id, assignments, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return user.Id;
    }

    public async Task<bool> UpdatePanelUserAsync(
        PanelUserEntity user,
        IReadOnlyCollection<RoleAssignment>? assignments,
        CancellationToken cancellationToken)
    {
        var current = await _db.PanelUser.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
        if (current is null) return false;

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        current.UserName = user.UserName;
        current.Email = user.Email;
        current.FullName = user.FullName;
        current.UserCode = user.UserCode;
        current.BranchCode = user.BranchCode;
        current.BranchName = user.BranchName;
        current.IsActive = user.IsActive;

        // رمز فقط وقتی عوض می‌شود که مقداری آمده باشد؛ هش‌کردن بیرون از این لایه است.
        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
            current.PasswordHash = user.PasswordHash;

        await _db.SaveChangesAsync(cancellationToken);

        // null یعنی «نقش‌ها را دست نزن»؛ فهرست خالی یعنی «همه را بردار».
        if (assignments is not null)
            await ReplaceUserAssignmentsAsync(current.Id, assignments, cancellationToken);
        else
            await BumpPermissionVersionAsync([current.Id], cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// انتساب‌های یک کاربر را کامل جایگزین می‌کند. فرم سند فهرست کامل را می‌فرستد،
    /// پس حذف‌وثبت دوباره با آنچه کاربر در صفحه می‌بیند یکی می‌ماند.
    /// </summary>
    private async Task ReplaceUserAssignmentsAsync(
        int panelUserId, IReadOnlyCollection<RoleAssignment> assignments, CancellationToken cancellationToken)
    {
        var current = await _db.PanelUserRole.Where(x => x.PanelUserId == panelUserId).ToListAsync(cancellationToken);
        _db.PanelUserRole.RemoveRange(current);
        await _db.SaveChangesAsync(cancellationToken);

        if (assignments.Count > 0)
        {
            // نقشی که وجود ندارد یا دامنه‌اش با AllowedScopeType نقش نمی‌خواند، بی‌صدا نمی‌افتد:
            // Handler قبل از صدا زدن این متد اعتبارسنجی می‌کند. این‌جا فقط نقش‌های موجود می‌مانند.
            var roles = await _db.Role
                .Where(r => assignments.Select(a => a.RoleId).Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            await _db.PanelUserRole.AddRangeAsync(
                assignments
                    .Where(a => roles.Contains(a.RoleId))
                    .Select(a => new PanelUserRoleEntity
                    {
                        PanelUserId = panelUserId,
                        RoleId = a.RoleId,
                        ScopeType = a.ScopeType,
                        ScopeId = a.ScopeType == AccessScopeType.Global ? null : a.ScopeId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByPanelUserId = a.CreatedByPanelUserId,
                    }),
                cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
        }

        await BumpPermissionVersionAsync([panelUserId], cancellationToken);
    }
}
