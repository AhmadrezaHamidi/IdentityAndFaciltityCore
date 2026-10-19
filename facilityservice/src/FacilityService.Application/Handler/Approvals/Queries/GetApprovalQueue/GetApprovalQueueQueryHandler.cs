using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Approvals.Queries.GetApprovalQueue;

public sealed class GetApprovalQueueQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : ApprovalHandlerBase(db, user, time), IRequestHandler<GetApprovalQueueQuery, FacilityBaseMessageResponse<FacilityPagedResponse<LoanRequestListItemDto>>>
{
    public async Task<FacilityBaseMessageResponse<FacilityPagedResponse<LoanRequestListItemDto>>> Handle(GetApprovalQueueQuery request, CancellationToken ct)
    {
        var query = ScopedLoans(ViewKey(request.Stage));
        if (request.CompanyId != null) query = query.Where(l => l.CompanyId == request.CompanyId);
        if (request.Status is { } s && !StageStatuses(request.Stage).Contains(s))
            throw Invalid("این وضعیت در صف این مرحله نیست.");
        var statuses = request.Status is { } st ? [st] : DefaultStatuses(request.Stage);
        query = query.Where(l => statuses.Contains(l.Status));
        var term = request.Term?.Trim();
        if (!string.IsNullOrEmpty(term))
            query = query.Where(l => Db.Personnel.Any(p => p.Id == l.PersonnelId && (p.FullName.Contains(term) || p.PersonnelCode.Contains(term))));

        var (page, size) = Paging(request.Page, request.PageSize);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(l => l.Status).ThenBy(l => l.CompanyId).ThenBy(l => l.PaymentOrder).ThenByDescending(l => l.Id)
            .Skip((page - 1) * size).Take(size).ToListAsync(ct);
        return Ok(Page(await LoanItemsAsync(items, ct), total, page, size));
    }
}
