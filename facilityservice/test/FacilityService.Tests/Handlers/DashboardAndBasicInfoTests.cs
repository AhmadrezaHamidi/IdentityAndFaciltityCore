using System.Text;

namespace FacilityService.Tests.Handlers;

/// <summary>US-A2 (اطلاعات پایه)، US-A4 (گزارش تغییرات)، US-B9/C3/D7 (داشبورد در دامنه).</summary>
public class DashboardAndBasicInfoTests
{
    [Fact]
    public async Task US_A2_basic_info_update_keeps_plan_ids_and_rejects_invalid_values()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var info = (await host.As("admin", new GetBasicInfoQuery(1))).Ok();
        Assert.Equal(30, info.UsableTable.Count);
        var qard12 = await host.PlanIdAsync(ContractType.QardHasan, 4, 12);

        (await host.As("admin", new UpdateBasicInfoCommand { SchemeId = 1, RepaymentTermsMonths = [12, 13], RowVersion = info.RowVersion })).Fails(400, "مضرب ۳");

        var table = info.UsableTable.Select(u => u.TermMonths == 12 && u.ContractType == ContractType.QardHasan ? u with { UsablePercent = 9 } : u)
            .Where(u => !(u.ContractType == ContractType.Murabaha && u.TermMonths == 60)).ToList();
        var saved = (await host.As("admin", new UpdateBasicInfoCommand { SchemeId = 1, MaxPlansPerCompany = 3, UsableTable = table, RowVersion = info.RowVersion })).Ok();
        Assert.Equal(3, saved.MaxPlansPerCompany);
        Assert.Equal(25, saved.UsableTable.Count);
        Assert.Equal(qard12, await host.PlanIdAsync(ContractType.QardHasan, 4, 12));   // شناسهٔ طرح ثابت می‌ماند
        Assert.Equal(25, (await host.As("admin", new GetPlansQuery(1))).Ok().Count);   // ردیف حذف‌شده غیرفعال
        Assert.Equal(4, saved.QardHasanRate);                                           // فیلد خالی تغییر نمی‌کند

        (await host.As("admin", new UpdateBasicInfoCommand { SchemeId = 1, MaxPlansPerCompany = 4, RowVersion = info.RowVersion })).Fails(409);
    }

    [Fact]
    public async Task Qard_hasan_rate_from_first_page_applies_to_every_qard_plan_like_the_prototype()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var info = (await host.As("admin", new GetBasicInfoQuery(1))).Ok();
        var qard12 = await host.PlanIdAsync(ContractType.QardHasan, 4, 12);

        var saved = (await host.As("admin", new UpdateBasicInfoCommand { SchemeId = 1, QardHasanRate = 3, RowVersion = info.RowVersion })).Ok();
        Assert.All(saved.UsableTable.Where(u => u.ContractType == ContractType.QardHasan), u => Assert.Equal(3, u.Rate));
        Assert.Equal(qard12, await host.PlanIdAsync(ContractType.QardHasan, 3, 12));   // همان طرح، شرکت‌ها و درخواست‌ها دست‌نخورده

        // ردیف قرض‌الحسنهٔ جدول با هر نرخی که بیاید، نرخ ثابت اطلاعات پایه را می‌گیرد
        var table = saved.UsableTable.Select(u => u.ContractType == ContractType.QardHasan ? u with { Rate = 1 } : u).ToList();
        var again = (await host.As("admin", new UpdateBasicInfoCommand { SchemeId = 1, UsableTable = table, RowVersion = saved.RowVersion })).Ok();
        Assert.All(again.UsableTable.Where(u => u.ContractType == ContractType.QardHasan), u => Assert.Equal(3, u.Rate));
        Assert.Equal(30, again.UsableTable.Count);

        // ترتیب طرح‌ها مثل buildPlanMatrix نمونه: قرض‌الحسنه، بعد هر نرخ مرابحه با همهٔ مدت‌ها
        var plans = (await host.As("admin", new GetPlansQuery(1))).Ok();
        Assert.Equal(await host.PlanIdAsync(ContractType.Murabaha, 5, 12) + 1, await host.PlanIdAsync(ContractType.Murabaha, 5, 24));
        Assert.Equal(30, plans.Count);
    }

    [Fact]
    public async Task Reasons_are_soft_deleted_and_hidden_from_dropdowns()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var created = (await host.As("admin", new CreateReasonCommand { Type = ReasonType.LoanCancel, Title = "بازنشستگی" })).Ok();
        Assert.Equal(3, (await host.As("manager", new GetReasonsQuery(ReasonType.LoanCancel))).Ok().Count);
        (await host.As("admin", new CreateReasonCommand { Type = ReasonType.LoanCancel, Title = "بازنشستگی" })).Fails(400, "وجود دارد");

        (await host.As("admin", new DeleteReasonCommand(created.Id))).Ok();
        Assert.Equal(2, (await host.As("manager", new GetReasonsQuery(ReasonType.LoanCancel))).Ok().Count);
        Assert.Equal(3, (await host.As("admin", new GetReasonsQuery(ReasonType.LoanCancel, IncludeInactive: true))).Ok().Count);
        Assert.Equal(2, (await host.As("manager", new GetReasonsQuery(ReasonType.LoanCancel, IncludeInactive: true))).Ok().Count);
    }

    [Fact]
    public async Task Dashboard_numbers_are_limited_to_user_scope()
    {
        await using var host = await FacilityTestHost.CreateAsync();

        var admin = (await host.As("admin", new GetDashboardQuery())).Ok();
        Assert.Equal(3, admin.Kpis.ActiveCompanies);
        Assert.Equal(3, admin.Kpis.TotalRequests);
        Assert.Equal(120_000_000, admin.Kpis.PendingAmount);
        Assert.Equal(["150002", "150003"], admin.CreditVsAllocated.Select(g => g.GroupId).OrderBy(x => x));   // سراسری ← به تفکیک شعبه

        var branch = (await host.As("branch", new GetDashboardQuery())).Ok();
        Assert.Equal(2, branch.Kpis.ActiveCompanies);
        Assert.Equal(90_000_000, branch.Kpis.PendingAmount);
        Assert.Equal(["شرکت آلفا", "شرکت بتا"], branch.CreditVsAllocated.Select(g => g.GroupTitle).OrderBy(x => x));

        var manager = (await host.As("manager3", new GetDashboardQuery())).Ok();
        Assert.Equal(1, manager.Kpis.ActiveCompanies);
        Assert.Equal(30_000_000, manager.Kpis.PendingAmount);
        // فیلتر شرکت دیگر دامنه را باز نمی‌کند
        Assert.Equal(0, (await host.As("manager3", new GetDashboardQuery { CompanyId = host.Alfa })).Ok().Kpis.TotalRequests);
    }

    [Fact]
    public async Task Removed_companies_ranking_and_csv_exports()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        (await host.As("admin", new DeleteCompanyCommand { Id = host.Beta, Description = "ثبت تکراری" })).Ok();

        var dashboard = (await host.As("admin", new GetDashboardQuery())).Ok();
        Assert.Equal("شرکت بتا", Assert.Single(dashboard.RemovedCompanies).CompanyName);

        var ranking = (await host.As("admin", new GetCompanyRankingQuery { SortBy = CompanyRankingSort.AverageBalance })).Ok();
        Assert.Equal(["شرکت آلفا", "شرکت گاما"], ranking.Select(r => r.CompanyName));
        Assert.Equal(1, ranking[0].Rank);

        var csv = (await host.As("branch", new ExportDashboardQuery(DashboardExport.LoanRequests))).Ok();
        var text = Encoding.UTF8.GetString(csv.Content);
        Assert.StartsWith("﻿", text);
        Assert.Contains("زهرا احمدی", text);
        Assert.DoesNotContain("امیر نوری", text);   // شعبهٔ دیگر

        var people = Encoding.UTF8.GetString((await host.As("digital", new ExportDashboardQuery(DashboardExport.Personnel))).Ok().Content);
        Assert.DoesNotContain("حسین رضایی", people); // در انتظار تأیید برای شعبهٔ دیجیتال پنهان است
        Assert.Contains("علی محمدی", people);
    }

    [Fact]
    public async Task US_A4_change_log_scope_restore_of_balances_and_csv()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var balances = (await host.As("branch", new GetCompanyBalancesQuery(host.Alfa))).Ok();
        (await host.As("branch", new UpdateCompanyBalancesCommand
        {
            CompanyId = host.Alfa, RowVersion = balances.RowVersion, Months = [new() { Month = balances.Months[0].Month, AverageBalance = 1_000 }],
        })).Ok();
        (await host.As("admin", new CreateReasonCommand { Type = ReasonType.LoanReject, Title = "سابقهٔ چک برگشتی" })).Ok();

        var log = (await host.As("branch", new SearchChangeLogsQuery())).Ok();
        Assert.Single(log.Items);                          // ثبت میانگین؛ نه تغییر اطلاعات پایه (بدون شرکت)
        Assert.False(log.Items[0].CanRestore);
        Assert.Empty((await host.As("branch2", new SearchChangeLogsQuery())).Ok().Items);
        Assert.Equal(2, (await host.As("digital", new SearchChangeLogsQuery())).Ok().TotalCount);

        var entry = (await host.As("admin", new SearchChangeLogsQuery { Category = ChangeLogCategory.Company, CompanyId = host.Alfa })).Ok().Items.Single();
        Assert.Contains("←", entry.Details);
        Assert.False(entry.CanRestore);                    // Q16: بازگردانی فعلاً ساخته نمی‌شود
        (await host.As("admin", new RestoreChangeCommand { Id = entry.Id, Description = "عدد اشتباه وارد شد" })).Fails(400, "فعلاً فعال نیست");
        Assert.Equal(1_000, (await host.As("branch", new GetCompanyBalancesQuery(host.Alfa))).Ok().Months[0].AverageBalance);
        Assert.Contains("←", Encoding.UTF8.GetString((await host.As("admin", new ExportChangeLogsQuery())).Ok().Content));
    }

    [Fact]
    public async Task Internal_lookup_lists_every_live_company_for_identity()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var all = (await host.As("admin", new InternalCompanyLookupQuery(null))).Ok();
        Assert.Equal(3, all.Count);
        Assert.Equal(host.Gamma.ToString(), (await host.As("admin", new InternalCompanyLookupQuery("گاما"))).Ok().Single().Id);
    }
}
