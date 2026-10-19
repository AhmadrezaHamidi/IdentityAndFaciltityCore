using FacilityService.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Tests.Handlers;

/// <summary>دادهٔ نمونهٔ اجرای محلی: SeedShowcaseAsync روی SeedAsync بدون خطا همهٔ وضعیت‌های درخواست را می‌سازد.</summary>
public class SampleDataTests
{
    [Fact]
    public async Task Showcase_seed_adds_every_loan_status()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var ids = new FacilityPanelSampleIds(host.Alfa, host.Beta, host.Gamma, host.People);
        await host.Db(async db =>
        {
            await FacilityPanelSampleData.SeedShowcaseAsync(db, ids, FacilityTestHost.Today, host.Time.GetUtcNow().UtcDateTime);
            return 0;
        });

        var statuses = await host.Db(db => db.LoanRequests.GroupBy(r => r.Status).Select(g => new { g.Key, Count = g.Count() }).ToListAsync());
        var byStatus = statuses.ToDictionary(s => s.Key, s => s.Count);
        Assert.Equal(1, byStatus[LoanRequestStatus.PendingManager]);
        Assert.Equal(2, byStatus[LoanRequestStatus.PendingCredit]);
        Assert.Equal(2, byStatus[LoanRequestStatus.Approved]);
        Assert.Equal(1, byStatus[LoanRequestStatus.RejectedByManager]);
        Assert.Equal(1, byStatus[LoanRequestStatus.RejectedByCredit]);
        Assert.Equal(1, byStatus[LoanRequestStatus.Cancelled]);
        Assert.Equal(14, await host.Db(db => db.Personnel.CountAsync()));
        Assert.Equal(9, await host.Db(db => db.ChangeLogs.CountAsync()));

        // درخواست‌های بتا در صف اعتبارات شعبهٔ ۱۵۰۰۰۲ دیده می‌شوند
        var queue = (await host.As("branch", new SearchLoanRequestsQuery { Page = 1, PageSize = 50 })).Ok();
        Assert.Contains(queue.Items, r => r.CompanyId == host.Beta);
    }
}
