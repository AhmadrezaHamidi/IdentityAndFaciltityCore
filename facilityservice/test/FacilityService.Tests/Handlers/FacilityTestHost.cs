#nullable enable
using FacilityService.Contracts.Persistence;
using FacilityService.Application.Authorization;
using FacilityService.Infrastructure;
using FacilityService.Infrastructure.DBContext;
using FacilityService.Infrastructure.Seed;
using FacilityService.Infrastructure.Services.Storage;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Tests.Handlers;

/// <summary>
/// میزبان تست Handlerها: SQLite در حافظه با همان مدل EF، MediatR واقعی با FacilityExceptionBehavior و
/// همان دادهٔ نمونهٔ بک‌اند شبیه‌سازی‌شدهٔ فرانت (frontend/src/mock/state.ts) تا هر دو یک رفتار را تست کنند.
/// «امروز» = ۱۴۰۵/۰۷/۲۰ (2026-10-12) مثل تست‌های فرانت.
/// با متغیر محیطی FACILITY_TEST_SQLSERVER (رشتهٔ اتصال به master) همین تست‌ها روی SQL Server واقعی اجرا می‌شوند؛
/// هر میزبان یک دیتابیس موقت می‌سازد و در پایان حذف می‌کند.
/// </summary>
public sealed class FacilityTestHost : IAsyncDisposable
{
    public static readonly DateOnly Today = new(2026, 10, 12);

    private readonly SqliteConnection? _connection;
    private readonly string? _sqlServer;

    /// <summary>رشتهٔ اتصال SQL Server برای اجرای تست‌ها روی دیتابیس واقعی؛ null یعنی SQLite.</summary>
    public static string? SqlServerMaster => Environment.GetEnvironmentVariable("FACILITY_TEST_SQLSERVER");
    private readonly ServiceProvider _provider;
    public FixedTime Time { get; } = new(new DateTimeOffset(2026, 10, 12, 6, 0, 0, TimeSpan.Zero));
    public InMemoryFileStorage Files { get; } = new();

    public long Alfa { get; private set; }
    public long Beta { get; private set; }
    public long Gamma { get; private set; }

    /// <summary>پرسنل نمونه به نام.</summary>
    public Dictionary<string, long> People { get; } = [];

    private FacilityTestHost()
    {
        if (SqlServerMaster is { } master)
            _sqlServer = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(master) { InitialCatalog = "FacilityTest_" + Guid.NewGuid().ToString("N")[..12] }.ConnectionString;
        else
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddFacilityPanelApplication();
        services.AddSingleton<TimeProvider>(Time);
        services.AddDbContext<FacilityPanelDbContext>(o =>
        {
            if (_sqlServer != null) o.UseSqlServer(_sqlServer, FacilityPanelModel.SqlServer);
            else o.UseSqlite(_connection!);
        });
        services.AddScoped<IFacilityPanelDbContext>(sp => sp.GetRequiredService<FacilityPanelDbContext>());
        services.AddSingleton<IFacilityFileStorage>(Files);
        services.AddScoped<TestCurrentUser>();
        services.AddScoped<IFacilityCurrentUser>(sp => sp.GetRequiredService<TestCurrentUser>());
        _provider = services.BuildServiceProvider();
    }

    public static async Task<FacilityTestHost> CreateAsync(bool seedCompanies = true)
    {
        var host = new FacilityTestHost();
        await using var scope = host._provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FacilityPanelDbContext>();
        // SQL Server: همان Migrationهای تولید (جداول + دادهٔ اولیه)؛ SQLite: ساخت از مدل + همان دادهٔ اولیه
        await scope.ServiceProvider.MigrateFacilityPanelDatabaseAsync();
        if (seedCompanies) await host.SeedCompaniesAsync(db);
        return host;
    }

    // ── کاربران نمونه (همان ماتریس پیش‌فرض فایل ۰۵) ──

    public static readonly string[] AdminKeys = typeof(K).GetFields().Select(f => (string)f.GetValue(null)!).ToArray();

    public static readonly Dictionary<string, string[]> RoleKeys = new()
    {
        ["admin"] = AdminKeys,
        ["digital"] =
        [
            K.CompaniesView, K.CompaniesDocumentsView, K.PersonnelView, K.LoansView, K.LoansCreditView, K.ManagerApprovalView,
            K.CreditApprovalView, K.DigitalApprovalDecide, K.DashboardView, K.DashboardExport, K.AuditView, K.AuditExport,
        ],
        ["branch"] =
        [
            K.CompaniesView, K.CompaniesCreate, K.CompaniesEdit, K.CompaniesBalanceEdit, K.CompaniesDocumentsView, K.CompaniesDocumentsUpload,
            K.CompaniesContractCancel, K.LoansView, K.LoansCreditView, K.CreditApprovalView, K.CreditApprovalApprove, K.CreditApprovalReject,
            K.CreditApprovalStage, K.DashboardView, K.DashboardExport, K.AuditView, K.AuditExport,
        ],
        ["manager"] =
        [
            K.PersonnelView, K.PersonnelCreate, K.PersonnelEdit, K.PersonnelDelete, K.PersonnelUpload, K.PersonnelApprove, K.LoansView,
            K.LoansCreditView, K.LoansCreate, K.LoansMembersEdit, K.LoansCancel, K.ManagerApprovalView, K.ManagerApprovalApprove,
            K.ManagerApprovalReject, K.DashboardView, K.DashboardExport, K.AuditView, K.AuditExport,
        ],
        ["hr"] =
        [
            K.PersonnelView, K.PersonnelCreate, K.PersonnelEdit, K.PersonnelDelete, K.PersonnelUpload, K.LoansView, K.LoansCreditView,
            K.LoansCreate, K.LoansMembersEdit,
        ],
    };

    /// <summary>نام کاربری ← (نقش، دامنه، شناسهٔ دامنه، شعبهٔ کاربر)</summary>
    private (string Role, PanelScopeType Scope, string? ScopeId, string Branch, int Id) Profile(string userName) => userName switch
    {
        "admin" => ("admin", PanelScopeType.Global, null, "150000", 1),
        "digital" => ("digital", PanelScopeType.Global, null, "150001", 2),
        "branch" => ("branch", PanelScopeType.Branch, "150002", "150002", 3),
        "branch2" => ("branch", PanelScopeType.Branch, "150003", "150003", 4),
        "manager" => ("manager", PanelScopeType.Company, Alfa.ToString(), "150002", 5),
        "hr.alfa" => ("hr", PanelScopeType.Company, Alfa.ToString(), "150002", 6),
        "manager3" => ("manager", PanelScopeType.Company, Gamma.ToString(), "150003", 7),
        _ => throw new ArgumentException(userName),
    };

    public FacilityPanelUser User(string userName, params PanelGrant[] extra)
    {
        var (role, scope, scopeId, branch, id) = Profile(userName);
        var grants = RoleKeys[role].Select(k => new PanelGrant(k, PanelPermissionEffect.Allow, scope, scopeId)).Concat(extra).ToList();
        return new FacilityPanelUser(id, userName, $"کاربر {userName}", $"U{id}", branch, $"شعبه {branch}", 1, new PanelPermissions(grants));
    }

    /// <summary>ارسال درخواست به‌نام یک کاربر (هر بار Scope و DbContext جدید، مثل یک درخواست HTTP).</summary>
    public async Task<TResponse> As<TResponse>(string userName, IRequest<TResponse> request) => await As(User(userName), request);

    public async Task<TResponse> As<TResponse>(FacilityPanelUser user, IRequest<TResponse> request)
    {
        await using var scope = _provider.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<TestCurrentUser>().Value = user;
        return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request);
    }

    public async Task<T> Db<T>(Func<FacilityPanelDbContext, Task<T>> query)
    {
        await using var scope = _provider.CreateAsyncScope();
        return await query(scope.ServiceProvider.GetRequiredService<FacilityPanelDbContext>());
    }

    public Task<int> PlanIdAsync(ContractType type, decimal rate, int term) =>
        Db(db => db.SchemePlans.Where(p => p.ContractType == type && p.Rate == rate && p.TermMonths == term).Select(p => p.Id).SingleAsync());

    public Task<int> ReasonIdAsync(ReasonType type) => Db(db => db.Reasons.Where(r => r.Type == type).OrderBy(r => r.Id).Select(r => r.Id).FirstAsync());

    // ── دادهٔ نمونه ──

    private async Task SeedCompaniesAsync(FacilityPanelDbContext db)
    {
        // همان دادهٔ نمونهٔ اجرای محلی (FacilityPanelSampleData)
        var ids = await FacilityPanelSampleData.SeedAsync(db, Today, Time.GetUtcNow().UtcDateTime);
        (Alfa, Beta, Gamma) = (ids.Alfa, ids.Beta, ids.Gamma);
        foreach (var (name, id) in ids.People) People[name] = id;
    }

    public async ValueTask DisposeAsync()
    {
        if (_sqlServer != null)
        {
            await using var scope = _provider.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<FacilityPanelDbContext>().Database.EnsureDeletedAsync();
        }
        await _provider.DisposeAsync();
        if (_connection != null) await _connection.DisposeAsync();
    }
}

public sealed class TestCurrentUser : IFacilityCurrentUser
{
    public FacilityPanelUser Value { get; set; } = null!;
}

public sealed class FixedTime(DateTimeOffset now) : TimeProvider
{
    public DateTimeOffset Now { get; set; } = now;
    public override DateTimeOffset GetUtcNow() => Now;
}

public sealed class InMemoryFileStorage : IFacilityFileStorage
{
    private readonly Dictionary<string, byte[]> _files = [];

    public int Count => _files.Count;

    public Task<string> PutAsync(string fileName, string contentType, byte[] content, CancellationToken ct)
    {
        var key = Guid.NewGuid().ToString("N");
        _files[key] = content;
        return Task.FromResult(key);
    }

    public Task<byte[]?> GetAsync(string storageKey, CancellationToken ct) => Task.FromResult(_files.GetValueOrDefault(storageKey));
}
