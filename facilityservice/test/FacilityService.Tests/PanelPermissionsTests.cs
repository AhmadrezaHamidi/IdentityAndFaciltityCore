using FacilityService.Application.Authorization;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Tests;

public class PanelPermissionsTests
{
    private static PanelGrant Allow(string key, PanelScopeType t = PanelScopeType.Global, string id = null) => new(key, PanelPermissionEffect.Allow, t, id);
    private static PanelGrant Deny(string key, PanelScopeType t = PanelScopeType.Global, string id = null) => new(key, PanelPermissionEffect.Deny, t, id);

    [Fact]
    public void Branch_user_can_only_act_on_own_branch_companies()
    {
        var p = new PanelPermissions([Allow(K.CompaniesView, PanelScopeType.Branch, "150002")]);

        Assert.True(p.Can(K.CompaniesView, FacilityResource.OfCompany("150002", "7")));
        Assert.False(p.Can(K.CompaniesView, FacilityResource.OfCompany("150003", "8")));
        Assert.False(p.Can(K.CompaniesView, FacilityResource.None));
        Assert.True(p.HasAnywhere(K.CompaniesView));
    }

    [Fact]
    public void Company_user_is_limited_to_own_company_even_in_same_branch()
    {
        var p = new PanelPermissions([Allow(K.PersonnelView, PanelScopeType.Company, "7")]);

        Assert.True(p.Can(K.PersonnelView, FacilityResource.OfCompany("150002", "7")));
        Assert.False(p.Can(K.PersonnelView, FacilityResource.OfCompany("150002", "9")));
    }

    [Fact]
    public void Deny_beats_allow_and_global_deny_removes_key()
    {
        var p = new PanelPermissions([Allow(K.LoansCancel), Deny(K.LoansCancel, PanelScopeType.Company, "7")]);
        Assert.False(p.Can(K.LoansCancel, FacilityResource.OfCompany("1", "7")));
        Assert.True(p.Can(K.LoansCancel, FacilityResource.OfCompany("1", "8")));

        var q = new PanelPermissions([Allow(K.LoansCancel), Deny(K.LoansCancel)]);
        Assert.False(q.HasAnywhere(K.LoansCancel));
    }

    [Fact]
    public void CanOn_helper_uses_company_branch_and_id()
    {
        var user = new FacilityPanelUser(1, "u", "نام", "U1", "150002", "شعبه", 3,
            new PanelPermissions([Allow(K.CreditApprovalApprove, PanelScopeType.Branch, "150002")]));

        Assert.True(user.CanOn(K.CreditApprovalApprove, "150002", 7L));
        Assert.False(user.CanOn(K.CreditApprovalApprove, "150009", 7L));
        Assert.Equal("150002", user.ToRegistrar().BranchCode);
    }
}

public class FacilityScopeFilterTests
{
    private sealed record Company(long Id, string BranchCode, string Name);

    private static readonly Company[] Data =
    [
        new(1, "B1", "a"), new(2, "B1", "b"), new(3, "B2", "c"), new(4, "B3", "d"),
    ];

    private static long[] Visible(params PanelGrant[] grants) =>
        new PanelPermissions(grants).ScopeFor(FacilityPermissionKeys.CompaniesView)
            .Apply(Data.AsQueryable(), c => c.BranchCode, c => c.Id.ToString())
            .Select(c => c.Id).OrderBy(x => x).ToArray();

    private static PanelGrant G(PanelScopeType t, string id, PanelPermissionEffect e = PanelPermissionEffect.Allow) =>
        new(FacilityPermissionKeys.CompaniesView, e, t, id);

    [Fact]
    public void Global_sees_all() => Assert.Equal(new long[] { 1, 2, 3, 4 }, Visible(G(PanelScopeType.Global, null)));

    [Fact]
    public void Branch_and_company_scopes_combine() =>
        Assert.Equal(new long[] { 1, 2, 4 }, Visible(G(PanelScopeType.Branch, "B1"), G(PanelScopeType.Company, "4")));

    [Fact]
    public void Deny_removes_rows_from_list() =>
        Assert.Equal(new long[] { 1, 3, 4 }, Visible(G(PanelScopeType.Global, null), G(PanelScopeType.Company, "2", PanelPermissionEffect.Deny)));

    [Fact]
    public void No_grant_or_global_deny_sees_nothing()
    {
        Assert.Empty(Visible());
        Assert.Empty(Visible(G(PanelScopeType.Global, null), G(PanelScopeType.Global, null, PanelPermissionEffect.Deny)));
    }
}

public class FacilityPermissionKeysParityTests
{
    private static string[] Keys(Type t) => t.GetFields()
        .Where(f => f.IsLiteral && f.FieldType == typeof(string))
        .Select(f => (string)f.GetRawConstantValue()).OrderBy(x => x).ToArray();

    [Fact]
    public void Every_key_is_seeded_in_sql_05()
    {
        // ریشهٔ ریپو (کنار facilityservice/): اسکریپت‌های Identity در identityservice/Database/ هستند، نه Database/ این سرویس
        var root = AppContext.BaseDirectory;
        while (root != null && !File.Exists(Path.Combine(root, "facilityservice", "FacilityService.sln"))) root = Path.GetDirectoryName(root);
        Assert.NotNull(root);
        var sql = File.ReadAllText(Path.Combine(root, "identityservice", "Database", "05_seed_facility_permissions.sql"));
        foreach (var key in Keys(typeof(FacilityPermissionKeys)))
            Assert.True(sql.Split('\n').Count(l => l.Contains($"'{key}'")) >= 2, $"{key} باید هم در کاتالوگ و هم در ماتریس ۰۵ باشد");
    }
}
