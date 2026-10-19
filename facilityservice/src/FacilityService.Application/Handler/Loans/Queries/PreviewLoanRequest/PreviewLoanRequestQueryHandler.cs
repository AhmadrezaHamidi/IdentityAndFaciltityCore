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

        var repeats = await RepeatAllocationsAsync(company.Id, request.Items.Select(i => i.PersonnelId).ToList(), credit.BaseDate, ct);
        var hints = preview.Messages.Select(m => new HintDto(m.Level, m.Text)).ToList();
        if (repeats.Count > 0)
            hints.Add(new HintDto(HintLevel.Warning,
                $"{PersianText.Number(repeats.Count)} نفر از افراد انتخاب‌شده در همین ماه قبلاً وام گرفته‌اند: "
                + string.Join("، ", repeats.Select(r => r.PersonnelName))
                + ". اگر تخصیص مجدد مورد نظرتان است، تأیید کنید."));

        return Ok(new LoanRequestPreviewDto(preview.SelectedCount, preview.Total, preview.RemainingCredit, preview.IsAllowed,
            preview.MaxCoverablePeople, preview.SuggestedMaxAmount, hints, repeats));
    }

    /// <summary>
    /// «هشدار تخصیص مجدد در همان ماه»: افرادی از فهرست انتخاب‌شده که در ماه شمسی تاریخ مبنا
    /// قبلاً درخواست وامی دارند که لغو یا رد نشده.
    ///
    /// ماه از BaseDate دستهٔ قبلی خوانده می‌شود، نه تاریخ ثبتش — چون همان BaseDate است که
    /// تعیین می‌کند اعتبار کدام ماه مصرف شده.
    /// </summary>
    private async Task<IReadOnlyList<RepeatAllocationDto>> RepeatAllocationsAsync(
        long companyId, IReadOnlyList<long> personnelIds, DateOnly baseDate, CancellationToken ct)
    {
        var ids = personnelIds.Distinct().Where(id => id > 0).ToList();
        if (ids.Count == 0) return [];

        var month = Jalali.MonthKey(baseDate);
        var rows = await (from l in Db.LoanRequests
                          join b in Db.LoanBatches on l.BatchId equals b.Id
                          where l.CompanyId == companyId && ids.Contains(l.PersonnelId)
                                && !LoanRequest.ReleasedStatuses.Contains(l.Status)
                          select new { l.PersonnelId, l.Amount, b.BaseDate }).ToListAsync(ct);

        var sameMonth = rows.Where(r => Jalali.MonthKey(r.BaseDate) == month)
            .GroupBy(r => r.PersonnelId)
            .ToDictionary(g => g.Key, g => g.Sum(r => r.Amount));
        if (sameMonth.Count == 0) return [];

        var people = await Db.Personnel.Where(p => sameMonth.Keys.Contains(p.Id))
            .Select(p => new { p.Id, p.FullName, p.PersonnelCode }).ToListAsync(ct);

        return people.Select(p => new RepeatAllocationDto(p.Id, p.FullName, p.PersonnelCode, month, sameMonth[p.Id]))
            .OrderBy(r => r.PersonnelName, StringComparer.Ordinal).ToList();
    }
}
