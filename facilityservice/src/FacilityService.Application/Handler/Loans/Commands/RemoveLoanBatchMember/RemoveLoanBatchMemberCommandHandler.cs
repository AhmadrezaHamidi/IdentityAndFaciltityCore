using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Loans.Commands.RemoveLoanBatchMember;

public sealed class RemoveLoanBatchMemberCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : LoanHandlerBase(db, user, time), IRequestHandler<RemoveLoanBatchMemberCommand, FacilityBaseMessageResponse<LoanBatchDto>>
{
    public async Task<FacilityBaseMessageResponse<LoanBatchDto>> Handle(RemoveLoanBatchMemberCommand request, CancellationToken ct)
    {
        var batch = await Db.LoanBatches.Include(b => b.Requests).ThenInclude(r => r.History).FirstOrDefaultAsync(b => b.Id == request.BatchId, ct)
                    ?? throw FacilityDomainException.NotFound("درخواست یافت نشد.");
        var company = await CompanyInScopeAsync(batch.CompanyId, ct, K.LoansMembersEdit);
        var loan = batch.Requests.FirstOrDefault(r => r.Id == request.RequestId) ?? throw FacilityDomainException.NotFound("درخواست یافت نشد.");
        var before = loan.Status;
        loan.RemoveFromBatch(Me, Now);
        Log(ChangeLogCategory.Loan, "حذف فرد از درخواست", $"{company.Name}: درخواست {PersianText.Number(loan.Id)}", company,
            new { status = before }, new { status = LoanRequestStatus.Cancelled });
        await Db.SaveChangesAsync(ct);
        return Ok(await BatchDtoAsync(batch, ct));
    }
}
