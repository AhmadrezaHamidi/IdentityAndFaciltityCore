using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Loans.Commands.CancelLoanRequest;

public sealed class CancelLoanRequestCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : LoanHandlerBase(db, user, time), IRequestHandler<CancelLoanRequestCommand, FacilityBaseMessageResponse<LoanRequestListItemDto>>
{
    public async Task<FacilityBaseMessageResponse<LoanRequestListItemDto>> Handle(CancelLoanRequestCommand request, CancellationToken ct)
    {
        var loan = await Db.LoanRequests.Include(l => l.History).FirstOrDefaultAsync(l => l.Id == request.Id, ct);
        if (loan == null || !CanOn(K.LoansCancel, loan)) throw FacilityDomainException.NotFound("درخواست یافت نشد.");
        var company = await CompanyInScopeAsync(loan.CompanyId, ct, K.LoansCancel);
        var reason = await ReasonAsync(request.ReasonId, ReasonType.LoanCancel, "انتخاب دلیل لغو الزامی است.", ct);
        var before = loan.Status;
        loan.Cancel(reason, request.Description, Me, Now);
        Log(ChangeLogCategory.Loan, "لغو درخواست وام", $"درخواست {PersianText.Number(loan.Id)}: {reason.Title}", company,
            new { status = before }, new { status = loan.Status });
        await Db.SaveChangesAsync(ct);
        return Ok((await LoanItemsAsync([loan], ct))[0]);
    }
}
