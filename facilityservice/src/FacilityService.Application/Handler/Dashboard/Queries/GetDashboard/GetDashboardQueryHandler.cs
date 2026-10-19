using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Dashboard.Queries.GetDashboard;

public sealed class GetDashboardQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : DashboardHandlerBase(db, user, time), IRequestHandler<GetDashboardQuery, FacilityBaseMessageResponse<DashboardDto>>
{
    public async Task<FacilityBaseMessageResponse<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken ct)
    {
        var d = await LoadAsync(request.CompanyId, request.BranchCode, request.From, request.To, ct);
        var live = d.Companies.Where(c => !c.IsDeleted).ToList();
        var pending = d.Loans.Where(l => LoanRequest.OpenStatuses.Contains(l.Status)).ToList();
        var approved = d.Loans.Where(l => l.Status == LoanRequestStatus.Approved).ToList();
        var allocated = d.Loans.Where(l => !LoanRequest.ReleasedStatuses.Contains(l.Status)).GroupBy(l => l.CompanyId).ToDictionary(g => g.Key, g => g.Sum(l => l.Amount));
        var plans = d.Settings.Values.SelectMany(s => s.Plans).ToDictionary(p => p.Id);

        long Created(Company c) => d.Settings.TryGetValue(c.SchemeId, out var s)
            ? CreditRules.Calculate(s, c, PlansOf(c, s), [], Today).Plans.Sum(p => p.CreatedCredit)
            : 0;

        var byBranch = Permissions.Can(K.DashboardView, FacilityResource.None);
        var groups = byBranch
            ? live.GroupBy(c => c.BranchCode).Select(g => (Id: g.Key, Title: $"شعبه {PersianText.Digits(g.Key)}", List: g.ToList()))
            : live.Select(c => (Id: c.Id.ToString(), Title: c.Name, List: new List<Company> { c }));

        var dto = new DashboardDto(
            new DashboardKpisDto(live.Count(c => c.Status == CompanyStatus.Active), d.Loans.Count, approved.Sum(l => l.Amount), pending.Sum(l => l.Amount)),
            new RequestStatusChartDto(approved.Count, pending.Count,
                d.Loans.Count(l => l.Status is LoanRequestStatus.RejectedByManager or LoanRequestStatus.RejectedByCredit or LoanRequestStatus.RejectedByDigital)),
            d.Loans.GroupBy(l => l.PlanId).Select(g => new PlanShareDto(g.Key, plans.TryGetValue(g.Key, out var p) ? p.Title : "", g.Count()))
                .OrderByDescending(x => x.RequestCount).ToList(),
            groups.Select(g => new CreditVsAllocatedDto(g.Id, g.Title, g.List.Sum(Created), g.List.Sum(c => allocated.GetValueOrDefault(c.Id)))).ToList(),
            byBranch ? DashboardGroupKind.Branch : DashboardGroupKind.Company,
            live.GroupBy(c => Jalali.MonthKey(c.ContractDate)).OrderBy(g => g.Key)
                .Select(g => new ContractsByMonthDto(g.Key, g.Count(), g.Select(c => c.Name).ToList())).ToList(),
            live.Select(c => new MonthlyAllocationDto(c.Id, c.Name,
                d.Loans.Where(l => l.CompanyId == c.Id && !LoanRequest.ReleasedStatuses.Contains(l.Status))
                    .GroupBy(l => Jalali.MonthKey(DateOnly.FromDateTime(l.CreatedAt)))
                    .ToDictionary(g => g.Key, g => g.Sum(l => l.Amount)))).ToList(),
            Removed(d, allocated));
        return Ok(dto);
    }
}
