using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Loans.Queries.GetLoanRequest;

public sealed class GetLoanRequestQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : LoanHandlerBase(db, user, time), IRequestHandler<GetLoanRequestQuery, FacilityBaseMessageResponse<LoanRequestDetailsDto>>
{
    public async Task<FacilityBaseMessageResponse<LoanRequestDetailsDto>> Handle(GetLoanRequestQuery request, CancellationToken ct)
    {
        var loan = await Db.LoanRequests.Include(l => l.History).Include(l => l.Documents).ThenInclude(d => d.File)
                       .FirstOrDefaultAsync(l => l.Id == request.Id, ct);
        if (loan == null || !new[] { K.LoansView, K.ManagerApprovalView, K.CreditApprovalView }.Any(k => CanOn(k, loan)) || !CanSeeStage(loan))
            throw FacilityDomainException.NotFound("درخواست یافت نشد.");
        var summary = (await LoanItemsAsync([loan], ct))[0];
        return Ok(new LoanRequestDetailsDto(summary, loan.Registrar.ToDto(),
            loan.History.OrderBy(h => h.At).ThenBy(h => h.Id).Select(h => new LoanRequestHistoryDto(h.From, h.To, h.Stage, h.Reason, h.Description, h.By.ToDto(), h.At)).ToList(),
            loan.Documents.Select(d => d.File.ToDto()).ToList()));
    }
}
