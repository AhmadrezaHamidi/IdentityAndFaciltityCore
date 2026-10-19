using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Loans.Queries.GetCompanyCredit;

public sealed class GetCompanyCreditQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : LoanHandlerBase(db, user, time), IRequestHandler<GetCompanyCreditQuery, FacilityBaseMessageResponse<CompanyCreditDto>>
{
    public async Task<FacilityBaseMessageResponse<CompanyCreditDto>> Handle(GetCompanyCreditQuery request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.CompanyId, ct, K.LoansCreditView);
        var credit = await CreditAsync(company, request.BaseDate ?? Today, ct);
        return Ok(new CompanyCreditDto(company.Id, credit.BaseDate,
            new EligibilityBannerDto(credit.Banner.Status, credit.Banner.Message, credit.Banner.AllowedDayOfMonth,
                credit.Banner.Status == EligibilityStatus.Allowed, [credit.Banner.Message]),
            credit.MonthRemainingCredit, credit.TotalAverageBase, credit.CurrentMonthAverage,
            credit.Plans.Select(p => new PlanCreditDto(p.Plan.Id, p.Plan.Title, p.Plan.ContractTitle, p.Plan.Rate, p.Plan.TermMonths,
                p.ShareOfAverage, p.Plan.UsablePercent, p.CreatedCredit, p.Allocated, p.RemainingThisMonth)).ToList(),
            // همان بازه‌ای که CreateLoanRequests اعتبارسنجی می‌کند: یک ماه شمسی گذشته تا امروز.
            Today, Jalali.AddMonths(Today, -1), Today));
    }
}
