using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Employees.Queries.SearchPersonnel;

public sealed class SearchPersonnelQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : PersonnelHandlerBase(db, user, time), IRequestHandler<SearchPersonnelQuery, FacilityBaseMessageResponse<FacilityPagedResponse<PersonnelListItemDto>>>
{
    public async Task<FacilityBaseMessageResponse<FacilityPagedResponse<PersonnelListItemDto>>> Handle(SearchPersonnelQuery request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.CompanyId, ct, K.PersonnelView, K.LoansCreate);
        var query = Db.Personnel.Where(p => p.CompanyId == company.Id && p.Status != PersonnelStatus.Deleted);
        if (!SeesUnapproved(company)) query = query.Where(p => p.Status == PersonnelStatus.Approved);
        if (request.Status != null) query = query.Where(p => p.Status == request.Status);
        var term = request.Term?.Trim();
        if (!string.IsNullOrEmpty(term))
            query = query.Where(p => p.FullName.Contains(term) || p.NationalCode.Contains(term) || p.Mobile.Contains(term) || p.PersonnelCode.Contains(term));
        if (request.HireDateFrom != null) query = query.Where(p => p.HireDate >= request.HireDateFrom);
        if (request.HireDateTo != null) query = query.Where(p => p.HireDate <= request.HireDateTo);
        if (request.OnlyAvailableForLoan)
            query = query.Where(p => p.Status == PersonnelStatus.Approved &&
                                     !Db.LoanRequests.Any(l => l.PersonnelId == p.Id && LoanRequest.OpenStatuses.Contains(l.Status)));

        var (page, size) = Paging(request.Page, request.PageSize, defaultSize: 100);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(p => p.Priority).ThenBy(p => p.Id).Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var locked = await LockedAsync(items.Select(p => p.Id).ToList(), ct);
        return Ok(Page(items.Select(p => p.ToListItem(locked.Contains(p.Id))).ToList(), total, page, size));
    }
}
