using IdentityService.Domain.Entity;

namespace IdentityService.Domain.Authorization;

/// <summary>
/// مجوزهای مؤثر یک کاربر پنل: (کلید ← دامنه‌ها).
///
/// زنجیره، همان زنجیرهٔ موجود پروژه است — جدول جدیدی لازم نیست:
///   PanelUser → PanelUserRole (+ دامنه) → Role → RolePanelMenu → PanelMenu.PermissionKey
///
/// یعنی «دسترسی را به نقش می‌دهیم و نقش را به کاربر»، و دامنه از انتساب می‌آید نه از نقش.
/// همان نقش «کاربر شعبه» برای صد کاربر در صد شعبه کار می‌کند.
/// </summary>
public sealed class EffectivePermissions
{
    private readonly Dictionary<string, List<AccessScope>> _allows;

    public int PermissionVersion { get; }

    private EffectivePermissions(Dictionary<string, List<AccessScope>> allows, int permissionVersion)
    {
        _allows = allows;
        PermissionVersion = permissionVersion;
    }

    public static EffectivePermissions Empty { get; } =
        new(new Dictionary<string, List<AccessScope>>(StringComparer.OrdinalIgnoreCase), 0);

    /// <summary>
    /// پیش‌نیاز بارگذاری (وگرنه خالی برمی‌گردد):
    ///   .Include(u =&gt; u.PanelUserRoles).ThenInclude(a =&gt; a.Role).ThenInclude(r =&gt; r.RolePanelMenus).ThenInclude(rm =&gt; rm.PanelMenu)
    /// </summary>
    public static EffectivePermissions Build(PanelUserEntity user)
    {
        var allows = new Dictionary<string, List<AccessScope>>(StringComparer.OrdinalIgnoreCase);

        // کاربر غیرفعال هیچ مجوزی ندارد — نه اینکه مجوزهایش را داشته باشد و جلوی ورودش گرفته شود.
        if (!user.IsActive)
            return new EffectivePermissions(allows, user.PermissionVersion);

        foreach (var assignment in user.PanelUserRoles)
        {
            if (assignment.Role is null || !assignment.Role.IsActive) continue;

            var scope = assignment.Scope;

            foreach (var link in assignment.Role.RolePanelMenus)
            {
                var menu = link.PanelMenu;
                if (menu is null || !menu.IsActive || string.IsNullOrWhiteSpace(menu.PermissionKey)) continue;

                Add(allows, menu.PermissionKey!, scope);
            }
        }

        return new EffectivePermissions(allows, user.PermissionVersion);
    }

    /// <summary>چک کامل روی منبع هدف — در Handler، بعد از اینکه منبع خوانده شد.</summary>
    public bool Can(string key, ResourceScope resource) =>
        _allows.TryGetValue(key, out var scopes) && scopes.Any(s => s.Covers(resource));

    /// <summary>
    /// گیت درشت (در فیلتر): کاربر این مجوز را در حداقل یک دامنه دارد؟
    /// چک واقعی روی منبع بعداً با <see cref="Can"/> انجام می‌شود.
    /// </summary>
    public bool HasAnywhere(string key) => _allows.TryGetValue(key, out var scopes) && scopes.Count > 0;

    public bool HasAnyOf(IEnumerable<string> keys) => keys.Any(HasAnywhere);

    /// <summary>دامنه‌های مجاز یک کلید — برای فیلتر Query قبل از صفحه‌بندی (معیار ۵).</summary>
    public IReadOnlyList<AccessScope> ScopesFor(string key) =>
        _allows.TryGetValue(key, out var scopes) ? scopes : [];

    /// <summary>همهٔ کلیدها — برای GET /Me تا فرانت دکمه‌های بی‌دسترسی را مخفی کند (معیار ۱).</summary>
    public IReadOnlyCollection<string> Keys => _allows.Keys;

    private static void Add(Dictionary<string, List<AccessScope>> map, string key, AccessScope scope)
    {
        if (!map.TryGetValue(key, out var list))
            map[key] = list = [];

        if (!list.Contains(scope))
            list.Add(scope);
    }
}
