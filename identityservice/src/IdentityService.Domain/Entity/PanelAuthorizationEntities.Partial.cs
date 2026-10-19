using IdentityService.Domain.Authorization;

namespace IdentityService.Domain.Entity;

// فیلدهای جدید مدل دسترسی، به‌صورت partial اضافه شده‌اند تا فایل‌های موجود Entity
// (که scaffold شده‌اند) دست‌نخورده بمانند و دوباره تولیدشان چیزی را پاک نکند.

public partial class PanelUserEntity
{
    /// <summary>فرم کاربر در سند: «نام کامل».</summary>
    public string? FullName { get; set; }

    /// <summary>فرم کاربر در سند: «کد کاربر».</summary>
    public string? UserCode { get; set; }

    /// <summary>فرم کاربر در سند: «کد شعبه». دامنهٔ پیش‌فرض انتساب نقش از همین پر می‌شود.</summary>
    public string? BranchCode { get; set; }

    /// <summary>فرم کاربر در سند: «نام شعبه». فقط نمایشی.</summary>
    public string? BranchName { get; set; }

    /// <summary>
    /// معیار پذیرش ۶: «در صورت هرگونه اعمال تغییر در سطح دسترسی کاربران، آن کاربر از سامانه
    /// لاگ‌اوت و ملزم به ورود با اطلاعات جدید گردد.»
    /// با هر تغییر دسترسی یک واحد بالا می‌رود. توکنی که نسخه‌اش از این کوچک‌تر باشد باطل است.
    /// </summary>
    public int PermissionVersion { get; set; } = 1;

    public DateTime? LastPermissionChangeAt { get; set; }
}

public partial class RoleEntity
{
    /// <summary>
    /// این نقش با چه نوع دامنه‌ای قابل انتساب است. «ادمین» فقط سراسری، «کاربر شعبه» فقط شعبه،
    /// «مدیر/منابع انسانی شرکت» فقط شرکت. جلوی انتساب بی‌معنا را می‌گیرد.
    /// </summary>
    public AccessScopeType AllowedScopeType { get; set; } = AccessScopeType.Global;

    /// <summary>۵ نقش سند. حذف و تغییر نامشان مجاز نیست؛ مجوزهایشان قابل تغییر است.</summary>
    public bool IsSystem { get; set; }
}

public partial class PanelUserRoleEntity
{
    /// <summary>سراسری / شعبه / شرکت.</summary>
    public AccessScopeType ScopeType { get; set; } = AccessScopeType.Global;

    /// <summary>کد شعبه یا شناسهٔ شرکت. برای دامنهٔ سراسری null است.</summary>
    public string? ScopeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedByPanelUserId { get; set; }

    /// <summary>دامنهٔ این انتساب، آماده برای EffectivePermissions.</summary>
    public AccessScope Scope => AccessScope.From(ScopeType, ScopeId);
}

public partial class PanelMenuEntity
{
    /// <summary>
    /// کلید پایدار مجوز، مثل facility.credit-approval.approve.
    /// همین است که در کد با [RequirePanelPermission(...)] به آن ارجاع می‌شود، پس عوض کردنش
    /// دسترسی را می‌شکند. برای ردیف‌های گروه‌بندی (Kind=0) خالی می‌ماند.
    /// </summary>
    public string? PermissionKey { get; set; }

    /// <summary>
    /// 0 = گروه/تب (در منو دیده می‌شود، خودش مجوز نیست)
    /// 1 = مشاهده (مجوز دیدن همان تب)
    /// 2 = عملیات (تأیید، رد، ویرایش، حذف — در منو دیده نمی‌شود)
    ///
    /// این همان «دسترسی به هر بخش و سرویس‌های آن بخش» سند است: بخش = 0، سرویس = 1 و 2.
    /// </summary>
    public byte Kind { get; set; }

    public int SortOrder { get; set; }
}
