using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Approvals.Queries.GetPendingCounts;

public sealed class GetPendingCountsQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : ApprovalHandlerBase(db, user, time), IRequestHandler<GetPendingCountsQuery, FacilityBaseMessageResponse<PendingCountsDto>>
{
    public async Task<FacilityBaseMessageResponse<PendingCountsDto>> Handle(GetPendingCountsQuery request, CancellationToken ct)
    {
        async Task<int?> Loans(string key, LoanRequestStatus status) =>
            Permissions.HasAnywhere(key) ? await ScopedLoans(key).CountAsync(l => l.Status == status, ct) : null;

        int? personnel = null;
        if (Permissions.HasAnywhere(K.PersonnelApprove))
        {
            var companies = ScopedCompanies(K.PersonnelApprove).Select(c => c.Id);
            personnel = await Db.Personnel.CountAsync(p => p.Status == PersonnelStatus.PendingManager && companies.Contains(p.CompanyId), ct);
        }
        return Ok(new PendingCountsDto(
            personnel,
            await Loans(K.ManagerApprovalView, LoanRequestStatus.PendingManager),
            await Loans(K.CreditApprovalView, LoanRequestStatus.PendingCredit),
            await Loans(K.DigitalApprovalDecide, LoanRequestStatus.PendingDigital)));
    }
}
