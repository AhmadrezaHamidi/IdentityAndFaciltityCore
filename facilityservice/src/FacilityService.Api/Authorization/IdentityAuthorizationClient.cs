using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FacilityService.Application.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace FacilityService.Api.Authorization;

/// <summary>
/// Facility ← Identity: توکن کاربر پنل را با POST Internal/Authorization/Resolve به کاربر و مجوزها تبدیل می‌کند.
/// نتیجه چند ثانیه با هش توکن کش می‌شود؛ پس تغییر دسترسی حداکثر با همین تأخیر در Facility اثر می‌کند.
/// </summary>
public interface IIdentityAuthorizationClient
{
    /// <summary>null یعنی نشست نامعتبر (توکن، کاربر غیرفعال، یا دسترسی تغییر کرده) ← 401.</summary>
    Task<FacilityPanelUser?> ResolveAsync(string token, CancellationToken cancellationToken);
}

public sealed class IdentityAuthorizationClient(HttpClient http, IMemoryCache cache, ILogger<IdentityAuthorizationClient> logger)
    : IIdentityAuthorizationClient
{
    public const string HttpClientName = "Identity";

    /// <summary>⚠ با نیاز «خروج فوری» (معیار ۶) هماهنگ کن؛ ۵ ثانیه یعنی تا ۵ ثانیه تأخیر.</summary>
    public static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(5);

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task<FacilityPanelUser?> ResolveAsync(string token, CancellationToken cancellationToken)
    {
        var key = "facility:panel-session:" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        if (cache.TryGetValue(key, out FacilityPanelUser? cached))
            return cached;

        HttpResponseMessage response;
        try
        {
            response = await http.PostAsJsonAsync("Internal/Authorization/Resolve", new { token }, Json, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            // Identity در دسترس نیست ← بسته (fail-closed)؛ کش نمی‌شود.
            logger.LogError(ex, "Identity authorization service is unreachable");
            throw;
        }

        if (!response.IsSuccessStatusCode)
        {
            cache.Set(key, (FacilityPanelUser?)null, CacheDuration);
            return null;
        }

        // ⚠ قالب پاسخ BaseMessageResponse واقعی Identity را چک کن (فرض: { isSuccess, statusCode, message, data }).
        var body = await response.Content.ReadFromJsonAsync<ResolveResponse>(Json, cancellationToken);
        var user = body is { IsSuccess: true, Data: not null } ? ToUser(body.Data) : null;
        cache.Set(key, user, CacheDuration);
        return user;
    }

    private static FacilityPanelUser ToUser(SessionData d) => new(
        d.PanelUserId, d.UserName, d.FullName, d.UserCode, d.BranchCode, d.BranchName, d.PermissionVersion,
        new PanelPermissions(d.Grants.Select(g => new PanelGrant(g.Key, g.Effect, g.ScopeType, g.ScopeId)).ToList()));

    private sealed record ResolveResponse(bool IsSuccess, string? Message, SessionData? Data);

    private sealed record SessionData(int PanelUserId, string UserName, string? FullName, string? UserCode,
        string? BranchCode, string? BranchName, int PermissionVersion, IReadOnlyList<GrantData> Grants);

    private sealed record GrantData(string Key, PanelPermissionEffect Effect, PanelScopeType ScopeType, string? ScopeId);
}

public sealed class FacilityCurrentUserAccessor(IHttpContextAccessor accessor) : IFacilityCurrentUser
{
    public const string HttpContextKey = "__FacilityPanelUser";

    public FacilityPanelUser Value =>
        accessor.HttpContext?.Items[HttpContextKey] as FacilityPanelUser
        ?? throw new InvalidOperationException("Endpoint باید [RequireFacilityPermission] داشته باشد.");
}
