using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Loans.Queries.SearchLoanRequests;

public sealed class SearchLoanRequestsQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : LoanHandlerBase(db, user, time), IRequestHandler<SearchLoanRequestsQuery, FacilityBaseMessageResponse<FacilityPagedResponse<LoanRequestListItemDto>>>
{
    public async Task<FacilityBaseMessageResponse<FacilityPagedResponse<LoanRequestListItemDto>>> Handle(SearchLoanRequestsQuery request, CancellationToken ct)
    {
        var query = ViewableLoans();
        if (request.CompanyId != null) query = query.Where(l => l.CompanyId == request.CompanyId);
        if (!string.IsNullOrWhiteSpace(request.BranchCode)) query = query.Where(l => l.BranchCode == request.BranchCode);
        if (request.Status != null) query = query.Where(l => l.Status == request.Status);
        if (request.PlanId != null) query = query.Where(l => l.PlanId == request.PlanId);
        if (request.From is { } from) query = query.Where(l => l.CreatedAt >= from.ToDateTime(TimeOnly.MinValue));
        if (request.To is { } to) query = query.Where(l => l.CreatedAt < to.AddDays(1).ToDateTime(TimeOnly.MinValue));
        var term = request.Term?.Trim();
        if (!string.IsNullOrEmpty(term))
            query = query.Where(l => Db.Personnel.Any(p => p.Id == l.PersonnelId && (p.FullName.Contains(term) || p.PersonnelCode.Contains(term) || p.NationalCode.Contains(term))));

        var (page, size) = Paging(request.Page, request.PageSize, defaultSize: 100);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(l => l.Id).Skip((page - 1) * size).Take(size).ToListAsync(ct);
        return Ok(Page(await LoanItemsAsync(items, ct), total, page, size));
    }
}
