using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Loans.Queries.PreviewLoanRequest;

public sealed class PreviewLoanRequestQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : LoanHandlerBase(db, user, time), IRequestHandler<PreviewLoanRequestQuery, FacilityBaseMessageResponse<LoanRequestPreviewDto>>
{
    public async Task<FacilityBaseMessageResponse<LoanRequestPreviewDto>> Handle(PreviewLoanRequestQuery request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.CompanyId, ct, K.LoansCreate);
        var credit = await CreditAsync(company, request.BaseDate ?? Today, ct);
        var plan = credit.Plans.FirstOrDefault(p => p.Plan.Id == request.PlanId) ?? throw Invalid("طرح انتخاب‌شده در تفاهم‌نامهٔ این شرکت نیست.");
        var preview = CreditRules.Preview(plan.RemainingThisMonth, request.Mode, request.Amount, request.Items.Select(i => i.Amount).ToList(),
            credit.Plans.Where(p => p.Plan.Id != plan.Plan.Id).ToList());
        return Ok(new LoanRequestPreviewDto(preview.SelectedCount, preview.Total, preview.RemainingCredit, preview.IsAllowed,
            preview.MaxCoverablePeople, preview.SuggestedMaxAmount, preview.Messages.Select(m => new HintDto(m.Level, m.Text)).ToList()));
    }
}
