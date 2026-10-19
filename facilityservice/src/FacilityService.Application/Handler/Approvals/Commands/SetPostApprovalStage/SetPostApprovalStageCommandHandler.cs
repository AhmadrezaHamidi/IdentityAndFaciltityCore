using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Approvals.Commands.SetPostApprovalStage;

public sealed class SetPostApprovalStageCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : ApprovalHandlerBase(db, user, time), IRequestHandler<SetPostApprovalStageCommand, FacilityBaseMessageResponse<LoanRequestListItemDto>>
{
    public async Task<FacilityBaseMessageResponse<LoanRequestListItemDto>> Handle(SetPostApprovalStageCommand request, CancellationToken ct)
    {
        var loan = await Db.LoanRequests.Include(l => l.History).FirstOrDefaultAsync(l => l.Id == request.RequestId, ct);
        if (loan == null || !CanOn(K.CreditApprovalStage, loan)) throw FacilityDomainException.NotFound("درخواست یافت نشد.");
        var company = await Db.Companies.FirstAsync(c => c.Id == loan.CompanyId, ct);
        Reason? reason = null;
        if (request.Stage == PostApprovalStage.RemoteGrantDenied)
            reason = await ReasonAsync(request.ReasonId, ReasonType.RemoteGrantDenied, "برای «عدم اعطای تسهیلات غیرحضوری» انتخاب دلیل الزامی است.", ct);
        var before = loan.PostStage;
        loan.SetPostStage(request.Stage, reason, string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(), Me, Now);
        Log(ChangeLogCategory.Loan, "تغییر مرحلهٔ پس از تأیید", $"درخواست {PersianText.Number(loan.Id)}", company, new { stage = before }, new { stage = loan.PostStage });
        await Db.SaveChangesAsync(ct);
        return Ok((await LoanItemsAsync([loan], ct))[0]);
    }
}
