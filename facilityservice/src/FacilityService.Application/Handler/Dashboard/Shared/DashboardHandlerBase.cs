using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Dashboard.Shared;

/// <summary>عضوهای مشترک Handlerهای این بخش (قبلاً در DashboardHandlers).</summary>
public abstract class DashboardHandlerBase(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time) : FacilityPanelHandlerBase(db, user, time)
{
    protected sealed record Data(List<Company> Companies, List<LoanRequest> Loans, Dictionary<int, SchemeSettings> Settings);

    /// <summary>شرکت‌های دامنه (حذف‌شده‌ها هم برای «شرکت‌های حذف‌شده») و درخواست‌های لغونشدهٔ آن‌ها.</summary>
    protected async Task<Data> LoadAsync(long? companyId, string? branchCode, DateOnly? from, DateOnly? to, CancellationToken ct)
    {
        var companies = Permissions.ScopeFor(K.DashboardView).Apply(Db.Companies.AsQueryable(), c => c.BranchCode, c => c.Id.ToString());
        if (companyId != null) companies = companies.Where(c => c.Id == companyId);
        if (!string.IsNullOrWhiteSpace(branchCode)) companies = companies.Where(c => c.BranchCode == branchCode);
        var list = await companies.Include(c => c.Plans).Include(c => c.Balances).Include(c => c.Cancellation).ToListAsync(ct);

        var ids = list.Select(c => c.Id).ToList();
        var loans = Db.LoanRequests.Where(l => ids.Contains(l.CompanyId) && l.Status != LoanRequestStatus.Cancelled);
        if (from is { } f) loans = loans.Where(l => l.CreatedAt >= f.ToDateTime(TimeOnly.MinValue));
        if (to is { } t) loans = loans.Where(l => l.CreatedAt < t.AddDays(1).ToDateTime(TimeOnly.MinValue));

        var schemeIds = list.Select(c => c.SchemeId).Distinct().ToList();
        var settings = await Db.SchemeSettings.Include(s => s.Plans).Where(s => schemeIds.Contains(s.SchemeId)).ToDictionaryAsync(s => s.SchemeId, ct);
        return new Data(list, await loans.ToListAsync(ct), settings);
    }

    protected static List<RemovedCompanyDto> Removed(Data d, IReadOnlyDictionary<long, long> allocated) =>
        d.Companies.Where(c => c.Status is CompanyStatus.Cancelled or CompanyStatus.Deleted or CompanyStatus.Expired).Select(c =>
        {
            var loans = d.Loans.Where(l => l.CompanyId == c.Id).ToList();
            var removedAt = c.DeletedAt ?? c.Cancellation?.CancelledAt ?? c.CreatedAt;
            var removedOn = DateOnly.FromDateTime(removedAt);
            return new RemovedCompanyDto(c.Id, c.Name, removedOn, CreditRules.LastAverage(c, removedOn),
                allocated.GetValueOrDefault(c.Id), loans.Count,
                loans.Count == 0 ? null : Math.Round(100m * loans.Count(l => l.Status == LoanRequestStatus.Approved) / loans.Count, 1),
                c.Status);
        }).ToList();
}
