using IdentityService.Domain.Authorization;
using IdentityService.Domain.Services.UserManagementPanel;
using Microsoft.AspNetCore.Http;
using OmidBank.Common.Cache;
using OmidBank.Common.Cookie;
using OmidBank.Common.JwtHelper;
using static OmidBank.Common.Enums;

namespace IdentityService.Infrastructure.Repositories.UserManagementPanel;

/// <summary>
/// توکن ← کاربر پنل با مجوزهای لحظه‌ای.
///
/// چهار چک، به ترتیب:
///   ۱. توکن در کوکی (یا هدر Bearer) هست؟
///   ۲. امضای توکن معتبر است؟
///   ۳. نشست در Redis هنوز زنده است؟ (خروج، همان کلید را پاک می‌کند)
///   ۴. کاربر فعال است و مجوز دارد؟
///
/// نتیجه برای مدت کوتاهی در حافظهٔ همین پروسه کش می‌شود تا هر درخواست یک کوئری با
/// چند Include نزند. عمداً در Redis کش نمی‌شود: مجوزها باید با یک Invalidate فوری
/// بروند و نگه‌داشتن snapshot در Redis یعنی همان چیزی که معیار ۶ از آن می‌گریزد.
///
/// تأخیر اعمال تغییر دسترسی = <see cref="CacheDuration"/>؛ و اگر تغییر از خود پنل
/// آمده باشد، <see cref="Invalidate"/> آن را به صفر می‌رساند.
/// </summary>
public sealed class PanelSessionResolver : IPanelSessionResolver
{
    /// <summary>سقف تأخیر اعمال تغییر دسترسی وقتی تغییر از جای دیگری آمده باشد.</summary>
    public static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, CacheEntry> Cache = new();

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwtService;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly IPanelAccessControlRepository _repository;

    public PanelSessionResolver(
        IHttpContextAccessor httpContextAccessor,
        IJwtService jwtService,
        IRedisCacheManager redisCacheManager,
        IPanelAccessControlRepository repository)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
        _redisCacheManager = redisCacheManager;
        _repository = repository;
    }

    public async Task<PanelSession?> ResolveAsync(CancellationToken cancellationToken)
    {
        var http = _httpContextAccessor.HttpContext;
        if (http is null) return null;

        var token = ReadToken(http);
        if (string.IsNullOrWhiteSpace(token)) return null;

        var validation = _jwtService.ValidateToken(token);
        if (!validation.Item1) return null;

        if (!int.TryParse(validation.Item3?.UserId?.ToString(), out var panelUserId) || panelUserId <= 0)
            return null;

        // خروج، کلید نشست را از Redis پاک می‌کند؛ پس توکنی که امضایش درست است ولی
        // نشستی ندارد، باطل است.
        if (_redisCacheManager.Read(string.Format(CacheKey.TosanUserSessionId, token)) is null)
            return null;

        if (Cache.TryGetValue(panelUserId, out var cached) && cached.ExpiresAt > DateTime.UtcNow)
            return cached.Session;

        var user = await _repository.GetPanelUserWithPermissionsAsync(panelUserId, cancellationToken);

        // کاربر حذف یا غیرفعال شده ← نشست باطل، نه «کاربر بدون مجوز».
        if (user is null || !user.IsActive)
        {
            Cache.TryRemove(panelUserId, out _);
            return null;
        }

        var session = new PanelSession(
            user.Id, user.UserName, user.FullName, user.UserCode, user.BranchCode, user.BranchName,
            EffectivePermissions.Build(user));

        Cache[panelUserId] = new CacheEntry(session, DateTime.UtcNow.Add(CacheDuration));
        return session;
    }

    public void Invalidate(int panelUserId) => Cache.TryRemove(panelUserId, out _);

    private static string? ReadToken(HttpContext http)
    {
        if (http.Request.Cookies.TryGetValue(CookieKey.Token, out var fromCookie) && !string.IsNullOrWhiteSpace(fromCookie))
            return fromCookie;

        var header = http.Request.Headers.Authorization.ToString();
        return header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? header["Bearer ".Length..].Trim()
            : null;
    }

    private sealed record CacheEntry(PanelSession Session, DateTime ExpiresAt);
}
