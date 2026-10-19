using FacilityService.Application.Authorization;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Tests.Handlers;

/// <summary>تب «مدیریت شرکت‌ها» روی Handlerهای واقعی + SQLite — US-A5، US-B1..B6.</summary>
public class CompanyStoryTests
{
    private static async Task<CreateCompanyCommand> NewCompany(FacilityTestHost host, string nid = "40404040404", params int[] plans) => new()
    {
        SchemeId = 1, Name = "شرکت دلتا", NationalId = nid, AgreementNumber = "NM-404", AccountNumber = "0100404040",
        ContractDate = new DateOnly(2026, 9, 1), AccountOpeningDate = new DateOnly(2026, 9, 1), SalaryFromSepah = true,
        PlanIds = plans.Length > 0 ? plans : [await host.PlanIdAsync(ContractType.QardHasan, 4, 12)],
    };

    [Fact]
    public async Task US_B1_branch_user_sees_only_own_branch_and_gets_404_for_others()
    {
        await using var host = await FacilityTestHost.CreateAsync();

        var list = (await host.As("branch", new SearchCompaniesQuery())).Ok();
        Assert.Equal(["شرکت بتا", "شرکت آلفا"], list.Page.Items.Select(c => c.Name));
        Assert.Equal(2, list.RegisteredCount);

        // دستکاری شناسه (معیار پذیرش ۴): شرکت شعبهٔ دیگر «یافت نشد»
        (await host.As("branch", new GetCompanyQuery(host.Gamma))).Fails(404);
        // فیلتر شعبه در Query دامنه را گسترده نمی‌کند
        Assert.Empty((await host.As("branch", new SearchCompaniesQuery { BranchCode = "150003" })).Ok().Page.Items);
        Assert.Equal(3, (await host.As("digital", new SearchCompaniesQuery())).Ok().RegisteredCount);
    }

    [Fact]
    public async Task US_B2_company_gets_registrar_and_branch_of_creator_and_plan_cap_is_enforced()
    {
        await using var host = await FacilityTestHost.CreateAsync();

        var created = (await host.As("branch", await NewCompany(host))).Ok();
        Assert.Equal("150002", created.BranchCode);
        Assert.Equal(3, created.Registrar.PanelUserId);
        Assert.Equal(CompanyStatus.Active, created.Status);

        // کاربر شعبه نمی‌تواند برای شعبهٔ دیگر ثبت کند (BranchCode نادیده گرفته می‌شود و شعبهٔ خودش ثبت می‌شود)
        var other = (await host.As("branch", await NewCompany(host, "50505050505") with { BranchCode = "150003" })).Ok();
        Assert.Equal("150002", other.BranchCode);
        // ادمین می‌تواند شعبه را تعیین کند
        Assert.Equal("150009", (await host.As("admin", await NewCompany(host, "60606060606") with { BranchCode = "150009" })).Ok().BranchCode);

        var plans = await host.Db(db => Task.FromResult(db.SchemePlans.Select(p => p.Id).Take(6).ToArray()));
        (await host.As("branch", await NewCompany(host, "70707070707", plans))).Fails(400, "حداکثر");
        (await host.As("branch", await NewCompany(host))).Fails(400, "شناسهٔ ملی");

        var log = await host.Db(db => Task.FromResult(db.ChangeLogs.OrderBy(l => l.Id).First(l => l.Action == "ثبت شرکت")));
        Assert.Equal(created.Id, log.CompanyId);
        Assert.Equal("150002", log.BranchCode);
    }

    [Fact]
    public async Task Company_roles_cannot_create_or_see_company_tab()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        // مدیر شرکت مجوز companies.view ندارد؛ Handler هم چیزی برنمی‌گرداند
        Assert.Empty((await host.As("manager", new SearchCompaniesQuery())).Ok().Page.Items);
        (await host.As("manager", await NewCompany(host))).Fails(403);
    }

    [Fact]
    public async Task US_B3_search_by_name_national_id_and_agreement()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        Assert.Equal("شرکت بتا", Assert.Single((await host.As("admin", new SearchCompaniesQuery { Term = "بتا" })).Ok().Page.Items).Name);
        Assert.Equal("شرکت گاما", Assert.Single((await host.As("admin", new SearchCompaniesQuery { Term = "303030" })).Ok().Page.Items).Name);
    }

    [Fact]
    public async Task US_B6_cancel_contract_requires_reason_description_and_letter_and_is_scoped()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var reason = await host.ReasonIdAsync(ReasonType.ContractCancel);
        var cmd = new CancelCompanyContractCommand
        {
            Id = host.Alfa, CancelDate = FacilityTestHost.Today, ReasonId = reason, Description = "به درخواست شرکت", LetterFile = TestData.Pdf(),
        };

        (await host.As("branch", cmd with { LetterFile = null })).Fails(400, "فایل");
        (await host.As("branch", cmd with { ReasonId = await host.ReasonIdAsync(ReasonType.LoanCancel) })).Fails(400, "دلیل");
        (await host.As("branch", cmd with { CancelDate = FacilityTestHost.Today.AddDays(1) })).Fails(400, "تاریخ");
        (await host.As("branch2", cmd)).Fails(404);
        (await host.As("digital", cmd)).Fails(403);

        var cancelled = (await host.As("branch", cmd)).Ok();
        Assert.Equal(CompanyStatus.Cancelled, cancelled.Status);
        Assert.Equal("عدم تمدید به درخواست شرکت", cancelled.Cancellation!.Reason);
        Assert.Equal("letter.pdf", cancelled.Cancellation.File!.FileName);
        (await host.As("branch", cmd)).Fails(400, "فعال نیست");
    }

    [Fact]
    public async Task US_B4_balances_are_saved_with_history_and_concurrency_check()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var balances = (await host.As("branch", new GetCompanyBalancesQuery(host.Beta))).Ok();
        Assert.Equal(12, balances.Months.Count);
        var month = balances.Months[1].Month;

        var saved = (await host.As("branch", new UpdateCompanyBalancesCommand
        {
            CompanyId = host.Beta, RowVersion = balances.RowVersion, Months = [new() { Month = month, AverageBalance = 2_000_000_000 }],
        })).Ok();
        Assert.Equal(2_000_000_000, saved.Months[1].AverageBalance);
        Assert.False(saved.Months[1].HasDocuments);
        Assert.NotNull(saved.Months[1].RemainingCredit);

        // نسخهٔ قدیمی ← 409
        (await host.As("branch", new UpdateCompanyBalancesCommand
        {
            CompanyId = host.Beta, RowVersion = balances.RowVersion, Months = [new() { Month = month, AverageBalance = 1 }],
        })).Fails(409);
        // ماه خارج از سال قرارداد
        (await host.As("branch", new UpdateCompanyBalancesCommand
        {
            CompanyId = host.Beta, RowVersion = saved.RowVersion, Months = [new() { Month = "1390/01", AverageBalance = 1 }],
        })).Fails(400, "سال جاری قرارداد");
        // شعبهٔ دیجیتال فقط مشاهده (US-C1)
        (await host.As("digital", new UpdateCompanyBalancesCommand
        {
            CompanyId = host.Beta, RowVersion = saved.RowVersion, Months = [new() { Month = month, AverageBalance = 1 }],
        })).Fails(403);
    }

    [Fact]
    public async Task US_B5_documents_upload_format_and_only_admin_deletes()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var type = await host.ReasonIdAsync(ReasonType.DocumentType);
        var month = (await host.As("branch", new GetCompanyBalancesQuery(host.Alfa))).Ok().Months[0].Month;

        (await host.As("branch", new UploadBalanceDocumentsCommand
        {
            CompanyId = host.Alfa, Month = month, DocumentTypeId = type, Files = [new([1, 2, 3, 4], "virus.exe", "application/octet-stream")],
        })).Fails(400, "فرمت");

        var docs = (await host.As("branch", new UploadBalanceDocumentsCommand
        {
            CompanyId = host.Alfa, Month = month, DocumentTypeId = type, Description = "بنکو تیر", Files = [TestData.Pdf("bank.pdf")],
        })).Ok();
        var doc = Assert.Single(docs);
        Assert.Equal("تصویر بنکو", doc.DocumentType);
        Assert.True((await host.As("branch", new GetCompanyBalancesQuery(host.Alfa))).Ok().Months[0].HasDocuments);

        var file = (await host.As("digital", new DownloadBalanceDocumentQuery(host.Alfa, doc.Id))).Ok();
        Assert.Equal("bank.pdf", file.FileName);

        (await host.As("branch", new DeleteBalanceDocumentCommand(host.Alfa, doc.Id))).Fails(404);
        Assert.True((await host.As("admin", new DeleteBalanceDocumentCommand(host.Alfa, doc.Id))).Ok());
    }

    [Fact]
    public async Task Edit_company_keeps_plans_with_active_loans_and_checks_row_version()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var alfa = (await host.As("branch", new GetCompanyQuery(host.Alfa))).Ok();
        var murabaha = await host.PlanIdAsync(ContractType.Murabaha, 5, 12);
        var update = new UpdateCompanyCommand
        {
            Id = host.Alfa, Name = "شرکت آلفا نو", AgreementNumber = alfa.AgreementNumber, AccountNumber = alfa.AccountNumber,
            ContractDate = alfa.ContractDate, AccountOpeningDate = alfa.AccountOpeningDate, SalaryFromSepah = true,
            PlanIds = [murabaha], RowVersion = alfa.RowVersion,
        };
        (await host.As("branch", update)).Fails(400, "درخواست وام فعال");

        var ok = (await host.As("branch", update with { PlanIds = alfa.Plans.Select(p => p.PlanId).ToList() })).Ok();
        Assert.Equal("شرکت آلفا نو", ok.Name);
        (await host.As("branch", update with { PlanIds = alfa.Plans.Select(p => p.PlanId).ToList() })).Fails(409);
    }

    [Fact]
    public async Task Delete_company_is_admin_only_and_blocked_by_approved_loans()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        (await host.As("branch", new DeleteCompanyCommand { Id = host.Beta, Description = "ثبت اشتباه" })).Fails(404);
        Assert.True((await host.As("admin", new DeleteCompanyCommand { Id = host.Beta, Description = "ثبت اشتباه" })).Ok());
        Assert.Equal(2, (await host.As("admin", new SearchCompaniesQuery())).Ok().RegisteredCount);
    }

    [Fact]
    public async Task Delete_company_is_blocked_while_it_has_loans_in_approval()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        (await host.As("admin", new DeleteCompanyCommand { Id = host.Alfa, Description = "ثبت اشتباه" })).Fails(400, "در جریان تأیید");
        Assert.Equal(3, (await host.As("admin", new SearchCompaniesQuery())).Ok().RegisteredCount);
    }

    [Fact]
    public async Task Lookup_unions_scopes_of_every_tab_key()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        Assert.Equal(["شرکت آلفا"], (await host.As("hr.alfa", new GetCompanyLookupQuery(null))).Ok().Select(x => x.Title));
        Assert.Equal(2, (await host.As("branch", new GetCompanyLookupQuery(null))).Ok().Count);

        // Deny روی یک شرکت، آن را از Dropdown حذف می‌کند
        var denied = host.User("branch", new PanelGrant(K.CompaniesView, PanelPermissionEffect.Deny, PanelScopeType.Company, host.Beta.ToString()),
            new PanelGrant(K.LoansView, PanelPermissionEffect.Deny, PanelScopeType.Company, host.Beta.ToString()),
            new PanelGrant(K.CreditApprovalView, PanelPermissionEffect.Deny, PanelScopeType.Company, host.Beta.ToString()),
            new PanelGrant(K.DashboardView, PanelPermissionEffect.Deny, PanelScopeType.Company, host.Beta.ToString()),
            new PanelGrant(K.AuditView, PanelPermissionEffect.Deny, PanelScopeType.Company, host.Beta.ToString()));
        Assert.Equal(["شرکت آلفا"], (await host.As(denied, new GetCompanyLookupQuery(null))).Ok().Select(x => x.Title));
    }
}
