namespace IdentityService.Domain.Authorization;

/// <summary>
/// دامنهٔ یک انتساب نقش: سراسری، یک شعبه، یا یک شرکت.
/// «منطق شخصی‌سازی داده‌ها»ی سند: ادمین و شعبهٔ دیجیتال سراسری، کاربر شعبه یک شعبه،
/// مدیر و منابع انسانی شرکت یک شرکت.
/// </summary>
public enum AccessScopeType : byte
{
    Global = 0,
    Branch = 1,
    Company = 2,
}

/// <summary>
/// دامنهٔ دسترسی. <see cref="Id"/> برای شعبه کد شعبه و برای شرکت شناسهٔ شرکت است.
/// روی انتساب نقش می‌نشیند، نه روی خود نقش: نقش «کاربر شعبه» یک بار تعریف می‌شود و
/// هر کاربر همان نقش را با شعبهٔ خودش می‌گیرد.
/// </summary>
public readonly record struct AccessScope
{
    public AccessScopeType Type { get; }
    public string? Id { get; }

    private AccessScope(AccessScopeType type, string? id)
    {
        Type = type;
        Id = id;
    }

    public static AccessScope Global { get; } = new(AccessScopeType.Global, null);

    public static AccessScope ForBranch(string branchCode) => new(AccessScopeType.Branch, Required(branchCode, "کد شعبه"));

    public static AccessScope ForCompany(string companyId) => new(AccessScopeType.Company, Required(companyId, "شناسهٔ شرکت"));

    public static AccessScope From(AccessScopeType type, string? id) => type switch
    {
        AccessScopeType.Global when string.IsNullOrWhiteSpace(id) => Global,
        AccessScopeType.Global => throw new InvalidOperationException("دامنهٔ سراسری شناسه ندارد."),
        AccessScopeType.Branch => ForBranch(id!),
        AccessScopeType.Company => ForCompany(id!),
        _ => throw new InvalidOperationException($"نوع دامنهٔ نامعتبر: {type}"),
    };

    /// <summary>
    /// آیا این دامنه منبع هدف را پوشش می‌دهد؟
    /// معیار پذیرش ۴ و ۵: منبعی که پوشش داده نشود مثل «پیدا نشد» رفتار می‌کند،
    /// حتی وقتی شناسه‌اش را دستی در URL گذاشته باشند.
    /// </summary>
    public bool Covers(ResourceScope resource) => Type switch
    {
        AccessScopeType.Global => true,
        AccessScopeType.Branch => resource.BranchCode is not null && resource.BranchCode == Id,
        AccessScopeType.Company => resource.CompanyId is not null && resource.CompanyId == Id,
        _ => false,
    };

    private static string Required(string? value, string name) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new InvalidOperationException($"{name} برای این دامنه الزامی است.")
            : value.Trim();

    public override string ToString() => Type == AccessScopeType.Global ? "Global" : $"{Type}:{Id}";
}

/// <summary>
/// شناسنامهٔ دامنه‌ای منبع هدف. یک شرکت هم کد شعبه دارد هم شناسهٔ خودش، پس هم کاربر آن
/// شعبه و هم کاربر آن شرکت پوشش داده می‌شوند.
/// برای منابع بی‌دامنه (نقش، کاربر پنل، اطلاعات پایه) از <see cref="None"/> استفاده کن؛
/// آن وقت فقط دامنهٔ سراسری پوشش می‌دهد.
/// </summary>
public readonly record struct ResourceScope(string? BranchCode, string? CompanyId)
{
    public static ResourceScope None { get; } = new(null, null);

    public static ResourceScope OfBranch(string branchCode) => new(branchCode, null);

    public static ResourceScope OfCompany(string? branchCode, string companyId) => new(branchCode, companyId);
}
