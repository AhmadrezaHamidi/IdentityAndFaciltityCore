namespace FacilityService.Application.Authorization;

public enum PanelScopeType : byte { Global = 0, Branch = 1, Company = 2 }

public enum PanelPermissionEffect : byte { Allow = 1, Deny = 2 }

/// <summary>یک مجوز با دامنه، همان‌طور که Identity در Internal/Authorization/Resolve برمی‌گرداند.</summary>
public sealed record PanelGrant(string Key, PanelPermissionEffect Effect, PanelScopeType ScopeType, string? ScopeId);

/// <summary>دامنهٔ منبع هدف: شرکت هم کد شعبه دارد هم شناسهٔ خودش.</summary>
public readonly record struct FacilityResource(string? BranchCode, string? CompanyId)
{
    public static FacilityResource None { get; } = new(null, null);
    public static FacilityResource OfCompany(string branchCode, string companyId) => new(branchCode, companyId);
}

/// <summary>
/// ارزیابی مجوز در Facility با همان قواعد Identity (EffectivePermissions):
/// مجوز روی منبع داده می‌شود اگر یک Allow آن را پوشش دهد و هیچ Deny آن را پوشش ندهد.
/// </summary>
public sealed class PanelPermissions(IReadOnlyList<PanelGrant> grants)
{
    public IReadOnlyList<PanelGrant> Grants { get; } = grants;

    public bool Can(string key, FacilityResource resource) =>
        Grants.Any(g => g.Effect == PanelPermissionEffect.Allow && Same(g.Key, key) && Covers(g, resource)) &&
        !Grants.Any(g => g.Effect == PanelPermissionEffect.Deny && Same(g.Key, key) && Covers(g, resource));

    /// <summary>کاربر این مجوز را در حداقل یک دامنه دارد؟ (چک اولیهٔ Attribute)</summary>
    public bool HasAnywhere(string key) =>
        Grants.Any(g => g.Effect == PanelPermissionEffect.Allow && Same(g.Key, key)) &&
        !Grants.Any(g => g.Effect == PanelPermissionEffect.Deny && Same(g.Key, key) && g.ScopeType == PanelScopeType.Global);

    /// <summary>دامنه‌های مجاز یک کلید برای فیلتر Query قبل از صفحه‌بندی.</summary>
    public FacilityScopeFilter ScopeFor(string key)
    {
        var allows = Grants.Where(g => g.Effect == PanelPermissionEffect.Allow && Same(g.Key, key)).ToList();
        var denies = Grants.Where(g => g.Effect == PanelPermissionEffect.Deny && Same(g.Key, key)).ToList();
        if (denies.Any(d => d.ScopeType == PanelScopeType.Global))
            return FacilityScopeFilter.Nothing;
        return new FacilityScopeFilter(
            allows.Any(a => a.ScopeType == PanelScopeType.Global),
            Ids(allows, PanelScopeType.Branch), Ids(allows, PanelScopeType.Company),
            Ids(denies, PanelScopeType.Branch), Ids(denies, PanelScopeType.Company));
    }

    private static IReadOnlyList<string> Ids(IEnumerable<PanelGrant> g, PanelScopeType t) =>
        g.Where(x => x.ScopeType == t && x.ScopeId != null).Select(x => x.ScopeId!).Distinct().ToList();

    private static bool Same(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    private static bool Covers(PanelGrant g, FacilityResource r) => g.ScopeType switch
    {
        PanelScopeType.Global => true,
        PanelScopeType.Branch => r.BranchCode != null && r.BranchCode == g.ScopeId,
        PanelScopeType.Company => r.CompanyId != null && r.CompanyId == g.ScopeId,
        _ => false,
    };
}

/// <summary>فیلتر دامنه برای لیست‌ها (معیار پذیرش ۴ و ۵: حتی با دستکاری URL دادهٔ دیگران نیاید).</summary>
public sealed record FacilityScopeFilter(
    bool All,
    IReadOnlyList<string> BranchCodes,
    IReadOnlyList<string> CompanyIds,
    IReadOnlyList<string> DeniedBranchCodes,
    IReadOnlyList<string> DeniedCompanyIds)
{
    public static FacilityScopeFilter Nothing { get; } = new(false, [], [], [], []);

    public bool IsEmpty => !All && BranchCodes.Count == 0 && CompanyIds.Count == 0;

    /// <summary>
    /// اعمال روی Query دیتابیس. branchCode و companyId باید ستون باشند (نه محاسبه در حافظه) تا EF ترجمه کند.
    /// مثال: companies.InScope(filter, c => c.BranchCode, c => c.Id.ToString())
    /// </summary>
    public IQueryable<T> Apply<T>(IQueryable<T> query,
        System.Linq.Expressions.Expression<Func<T, string?>> branchCode,
        System.Linq.Expressions.Expression<Func<T, string?>> companyId)
    {
        if (IsEmpty) return query.Where(_ => false);

        var p = branchCode.Parameters[0];
        var company = new ReplaceParameter(companyId.Parameters[0], p).Visit(companyId.Body)!;
        var branch = branchCode.Body;

        System.Linq.Expressions.Expression allowed = System.Linq.Expressions.Expression.Constant(All);
        if (!All)
        {
            allowed = System.Linq.Expressions.Expression.OrElse(In(branch, BranchCodes), In(company, CompanyIds));
        }
        var notDenied = System.Linq.Expressions.Expression.Not(
            System.Linq.Expressions.Expression.OrElse(In(branch, DeniedBranchCodes), In(company, DeniedCompanyIds)));

        var body = System.Linq.Expressions.Expression.AndAlso(allowed, notDenied);
        return query.Where(System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(body, p));
    }

    private static System.Linq.Expressions.Expression In(System.Linq.Expressions.Expression value, IReadOnlyList<string> list)
    {
        if (list.Count == 0) return System.Linq.Expressions.Expression.Constant(false);
        var contains = typeof(Enumerable).GetMethods()
            .First(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(string));
        return System.Linq.Expressions.Expression.Call(contains, System.Linq.Expressions.Expression.Constant(list.ToList()), value);
    }

    private sealed class ReplaceParameter(System.Linq.Expressions.ParameterExpression from, System.Linq.Expressions.ParameterExpression to)
        : System.Linq.Expressions.ExpressionVisitor
    {
        protected override System.Linq.Expressions.Expression VisitParameter(System.Linq.Expressions.ParameterExpression node) =>
            node == from ? to : base.VisitParameter(node);
    }
}
