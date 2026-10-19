#nullable enable
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FacilityService.Api.Authorization;
using FacilityService.Contracts.Persistence;
using FacilityService.Application.Authorization;
using FacilityService.Infrastructure;
using FacilityService.Infrastructure.DBContext;
using FacilityService.Infrastructure.Seed;
using FacilityService.Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FacilityService.Tests.Handlers;

/// <summary>
/// از HTTP تا دیتابیس: کنترلرهای واقعی، [RequireFacilityPermission]، Model Binding (JSON و multipart)، MediatR و EF روی SQLite.
/// Identity با یک کلاینت جعلی جایگزین شده (توکن = نام کاربر). جریان: ثبت شرکت ← میانگین ← معرفی پرسنل ← تأیید ← درخواست وام ← تأیید مدیر.
/// </summary>
public class HttpPipelineTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private WebApplicationFactory<Program> _factory = null!;
    private readonly FakeIdentity _identity = new();
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Testing");
            b.UseSetting("ConnectionStrings:FacilityPanel", "Data Source=unused");
            b.UseSetting("FacilityPanel:Provider", "Sqlite");
            b.ConfigureTestServices(s =>
            {
                s.RemoveAll<DbContextOptions<FacilityPanelDbContext>>();
                s.AddDbContext<FacilityPanelDbContext>(o => o.UseSqlite(_connection));
                s.RemoveAll<IIdentityAuthorizationClient>();
                s.AddSingleton<IIdentityAuthorizationClient>(_identity);
                s.RemoveAll<IFacilityFileStorage>();
                s.AddSingleton<IFacilityFileStorage>(new InMemoryFileStorage());
                s.RemoveAll<TimeProvider>();
                s.AddSingleton<TimeProvider>(new FixedTime(new DateTimeOffset(2026, 10, 12, 6, 0, 0, TimeSpan.Zero)));
            });
        });
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FacilityPanelDbContext>();
        await db.Database.EnsureCreatedAsync();
        await FacilityPanelSeed.EnsureSeedAsync(db);
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private HttpClient As(string token)
    {
        var c = _factory.CreateClient();
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return c;
    }

    private static async Task<JsonElement> Data(HttpResponseMessage r, HttpStatusCode expected = HttpStatusCode.OK)
    {
        var body = await r.Content.ReadAsStringAsync();
        Assert.True(r.StatusCode == expected, $"{(int)r.StatusCode}: {body}");
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.TryGetProperty("data", out var d) ? d.Clone() : doc.RootElement.Clone();
    }

    [Fact]
    public async Task Company_to_loan_flow_over_http_with_permission_filter()
    {
        _identity.Add("admin", "admin", PanelScopeType.Global, null, "150000");
        _identity.Add("branch", "branch", PanelScopeType.Branch, "150002", "150002");

        // بدون توکن ← 401؛ کلید ندارد ← 403 (قبل از Handler)
        Assert.Equal(HttpStatusCode.Unauthorized, (await _factory.CreateClient().GetAsync("Companies")).StatusCode);
        _identity.Add("nobody", "hr", PanelScopeType.Company, "999", "1");
        Assert.Equal(HttpStatusCode.Forbidden, (await As("nobody").GetAsync("Companies")).StatusCode);

        // ثبت شرکت با فرم multipart و فایل تفاهم‌نامه
        var plans = await Data(await As("branch").GetAsync("BasicInfo/1/Plans"));
        var qard12 = plans.EnumerateArray().First(p => p.GetProperty("contractType").GetInt32() == 1 && p.GetProperty("termMonths").GetInt32() == 12).GetProperty("id").GetInt32();
        var contract = Jalali.AddMonths(FacilityTestHost.Today, -3);
        var form = new MultipartFormDataContent
        {
            { new StringContent("1"), "SchemeId" },
            { new StringContent("شرکت آلفا"), "Name" },
            { new StringContent("10101010101"), "NationalId" },
            { new StringContent("NM-101"), "AgreementNumber" },
            { new StringContent("0100123456"), "AccountNumber" },
            { new StringContent(contract.ToString("yyyy-MM-dd")), "ContractDate" },
            { new StringContent(contract.ToString("yyyy-MM-dd")), "AccountOpeningDate" },
            { new StringContent("true"), "SalaryFromSepah" },
            { new StringContent(qard12.ToString()), "PlanIds" },
            { new ByteArrayContent(new byte[2048]) { Headers = { ContentType = new("application/pdf") } }, "agreementFile", "agreement.pdf" },
        };
        var company = await Data(await As("branch").PostAsync("Companies", form));
        var companyId = company.GetProperty("id").GetInt64();
        Assert.Equal("150002", company.GetProperty("branchCode").GetString());
        Assert.Equal("agreement.pdf", company.GetProperty("agreementFile").GetProperty("fileName").GetString());

        // اعتبارسنجی DataAnnotations در کنترلر (دلیل و توضیح لغو خالی) ← 400 قبل از Handler
        var emptyCancel = await As("branch").PostAsync($"Companies/{companyId}/Cancel", new MultipartFormDataContent { { new StringContent("x"), "Description" } });
        Assert.True(emptyCancel.StatusCode == HttpStatusCode.BadRequest, await emptyCancel.Content.ReadAsStringAsync());

        // میانگین موجودی
        var balances = await Data(await As("branch").GetAsync($"Companies/{companyId}/Balances"));
        var month = balances.GetProperty("months")[0].GetProperty("month").GetString();
        await Data(await As("branch").PutAsJsonAsync($"Companies/{companyId}/Balances",
            new { rowVersion = balances.GetProperty("rowVersion").GetString(), months = new[] { new { month, averageBalance = 5_000_000_000L } } }));

        // نقش‌های شرکت حالا که Id شرکت معلوم است
        _identity.Add("hr", "hr", PanelScopeType.Company, companyId.ToString(), "150002");
        _identity.Add("manager", "manager", PanelScopeType.Company, companyId.ToString(), "150002");
        Assert.Equal(HttpStatusCode.NotFound, (await As("hr").GetAsync($"Companies/{companyId + 1}/Credit")).StatusCode);

        var person = await Data(await As("hr").PostAsJsonAsync($"Companies/{companyId}/Personnel", new
        {
            fullName = "علی محمدی", nationalCode = "0012345679", mobile = "09121234567", personnelCode = "P1", hireDate = "2023-04-10", priority = 1,
        }));
        var personId = person.GetProperty("id").GetInt64();
        Assert.Equal(1, person.GetProperty("status").GetInt32());

        var decided = await Data(await As("manager").PostAsJsonAsync("Personnel/Decisions", new { personnelIds = new[] { personId }, decision = 1 }));
        Assert.Equal(1, decided.GetInt32());

        var credit = await Data(await As("hr").GetAsync($"Companies/{companyId}/Credit"));
        Assert.Equal(3, credit.GetProperty("banner").GetProperty("status").GetInt32());
        Assert.Equal(440_000_000, credit.GetProperty("plans")[0].GetProperty("createdCredit").GetInt64()); // ۸٪ × ۱٫۱ × ۵ میلیارد

        var batch = await Data(await As("hr").PostAsJsonAsync($"Companies/{companyId}/LoanRequests", new
        {
            planId = qard12, baseDate = FacilityTestHost.Today.ToString("yyyy-MM-dd"), mode = 1, amount = 100_000_000, items = new[] { new { personnelId = personId } },
        }));
        var requestId = batch.GetProperty("requests")[0].GetProperty("id").GetInt64();

        // تأیید/رد به کلید متفاوت نیاز دارد: منابع انسانی ← 403
        Assert.Equal(HttpStatusCode.Forbidden, (await As("hr").PostAsJsonAsync("ManagerApprovals/Decisions", new { requestIds = new[] { requestId }, decision = 1 })).StatusCode);
        var decision = await Data(await As("manager").PostAsJsonAsync("ManagerApprovals/Decisions", new { requestIds = new[] { requestId }, decision = 1 }));
        Assert.Equal(1, decision.GetProperty("processed").GetInt32());

        // تصمیم اعتبارات با فرم و پیوست
        var creditForm = new MultipartFormDataContent
        {
            { new StringContent(requestId.ToString()), "RequestIds" },
            { new StringContent("1"), "Decision" },
            { new ByteArrayContent(new byte[2048]) { Headers = { ContentType = new("application/pdf") } }, "attachment", "approval.pdf" },
        };
        var creditDecision = await Data(await As("branch").PostAsync("CreditApprovals/Decisions", creditForm));
        Assert.Equal(1, creditDecision.GetProperty("processed").GetInt32());

        var details = await Data(await As("admin").GetAsync($"LoanRequests/{requestId}"));
        Assert.Equal(7, details.GetProperty("summary").GetProperty("status").GetInt32());
        Assert.Equal("approval.pdf", details.GetProperty("documents")[0].GetProperty("fileName").GetString());

        // قاعدهٔ کسب‌وکار ← 400 با پیام فارسی از FacilityExceptionBehavior (مانده ۳۴۰ میلیون است)
        var over = await As("hr").PostAsJsonAsync($"Companies/{companyId}/LoanRequests", new
        {
            planId = qard12, baseDate = FacilityTestHost.Today.ToString("yyyy-MM-dd"), mode = 1, amount = 400_000_000, items = new[] { new { personnelId = personId } },
        });
        Assert.Equal(HttpStatusCode.BadRequest, over.StatusCode);
        Assert.Contains("بیشتر از اعتبار", await over.Content.ReadAsStringAsync());

        // خروجی CSV به‌صورت فایل
        var csv = await As("branch").GetAsync("Dashboard/Exports/LoanRequests");
        Assert.Equal("text/csv", csv.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Errors_have_one_persian_shape_and_identity_outage_is_503()
    {
        _identity.Add("manager", "manager", PanelScopeType.Company, "1", "150002");

        // ورودی نامعتبر (ModelState) ← 400 با statusCode/message فارسی و فهرست فیلدها، نه ProblemDetails انگلیسی
        var invalid = await As("manager").PostAsJsonAsync("ManagerApprovals/Decisions", new { requestIds = Array.Empty<long>(), decision = 1 });
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        var body = JsonDocument.Parse(await invalid.Content.ReadAsStringAsync()).RootElement;
        Assert.Equal(400, body.GetProperty("statusCode").GetInt32());
        Assert.StartsWith("اطلاعات ورودی معتبر نیست", body.GetProperty("message").GetString());
        Assert.True(body.GetProperty("errors").TryGetProperty("requestIds", out _));

        // Identity در دسترس نیست ← 503 با پیام فارسی (fail-closed)
        _identity.Down = true;
        var down = await As("manager").GetAsync("Companies");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, down.StatusCode);
        Assert.Contains("سرویس احراز هویت در دسترس نیست", await down.Content.ReadAsStringAsync());
    }

    private sealed class FakeIdentity : IIdentityAuthorizationClient
    {
        private readonly Dictionary<string, FacilityPanelUser> _users = [];

        public bool Down { get; set; }

        public void Add(string token, string role, PanelScopeType scope, string? scopeId, string branch) =>
            _users[token] = new FacilityPanelUser(_users.Count + 1, token, token, "U" + token, branch, "شعبه " + branch, 1,
                new PanelPermissions(FacilityTestHost.RoleKeys[role].Select(k => new PanelGrant(k, PanelPermissionEffect.Allow, scope, scopeId)).ToList()));

        public Task<FacilityPanelUser?> ResolveAsync(string token, CancellationToken ct) =>
            Down ? throw new HttpRequestException("identity down") : Task.FromResult(_users.GetValueOrDefault(token));
    }
}
