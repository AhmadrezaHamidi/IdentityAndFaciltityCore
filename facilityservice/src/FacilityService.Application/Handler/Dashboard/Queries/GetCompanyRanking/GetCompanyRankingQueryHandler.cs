using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Dashboard.Queries.GetCompanyRanking;

public sealed class GetCompanyRankingQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : DashboardHandlerBase(db, user, time), IRequestHandler<GetCompanyRankingQuery, FacilityBaseMessageResponse<IReadOnlyList<CompanyRankingDto>>>
{
    public async Task<FacilityBaseMessageResponse<IReadOnlyList<CompanyRankingDto>>> Handle(GetCompanyRankingQuery request, CancellationToken ct)
    {
        var d = await LoadAsync(null, null, null, null, ct);
        var live = d.Companies.Where(c => !c.IsDeleted).ToList();
        var ids = live.Select(c => c.Id).ToList();
        var personnel = await Db.Personnel.Where(p => ids.Contains(p.CompanyId) && p.Status != PersonnelStatus.Deleted)
            .GroupBy(p => p.CompanyId).Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count, ct);

        var rows = live.Select(c =>
        {
            var loans = d.Loans.Where(l => l.CompanyId == c.Id && !LoanRequest.ReleasedStatuses.Contains(l.Status)
                                           && (request.Month == null || Jalali.MonthKey(DateOnly.FromDateTime(l.CreatedAt)) == request.Month)).ToList();
            var average = request.Month == null ? CreditRules.LastAverage(c, Today) : c.Balances.FirstOrDefault(b => b.Month == request.Month)?.Amount;
            return (Company: c, Average: average, Allocated: loans.Sum(l => l.Amount), Requests: loans.Count, Personnel: personnel.GetValueOrDefault(c.Id));
        });
        rows = request.SortBy switch
        {
            CompanyRankingSort.AllocatedAmount => rows.OrderByDescending(r => r.Allocated),
            CompanyRankingSort.RequestCount => rows.OrderByDescending(r => r.Requests),
            CompanyRankingSort.PersonnelCount => rows.OrderByDescending(r => r.Personnel),
            _ => rows.OrderByDescending(r => r.Average ?? -1),
        };
        return Ok<IReadOnlyList<CompanyRankingDto>>(rows.Take(Math.Clamp(request.Top, 1, 500)).Select((r, i) =>
            new CompanyRankingDto(i + 1, r.Company.Id, r.Company.Name, r.Average, r.Allocated, r.Requests, r.Personnel, StatusTitles.Of(r.Company.Status))).ToList());
    }
}
