using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Commands.UpdateBasicInfo;

public sealed class UpdateBasicInfoCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : FacilityPanelHandlerBase(db, user, time), IRequestHandler<UpdateBasicInfoCommand, FacilityBaseMessageResponse<BasicInfoDto>>
{
    public async Task<FacilityBaseMessageResponse<BasicInfoDto>> Handle(UpdateBasicInfoCommand request, CancellationToken ct)
    {
        var settings = await SettingsAsync(request.SchemeId, ct);
        settings.EnsureRowVersion(request.RowVersion);
        var before = Snapshot(settings);
        settings.Update(
            request.ContractTypes, request.QardHasanRate, request.MurabahaRates, request.RepaymentTermsMonths,
            request.SalaryFromSepahMultiplierYes, request.SalaryFromSepahMultiplierNo, request.ContractDurationMonths,
            request.GraceMonthsAfterExpiry, request.MaxPlansPerCompany, request.PersonnelIntroductionDelayMonths,
            request.WaitingPeriodDays, request.DigitalApprovalEnabled,
            request.UsableTable?.Select(u => new UsablePercentRow(u.TermMonths, u.ContractType, u.Rate, u.UsablePercent)).ToList());
        var scheme = await Db.Schemes.FirstAsync(s => s.Id == request.SchemeId, ct);
        Log(ChangeLogCategory.Company, "ویرایش اطلاعات پایه", $"تنظیمات طرح {scheme.Title}", null, before, Snapshot(settings));
        await Db.SaveChangesAsync(ct);
        return Ok(settings.ToDto());
    }

    /// <summary>همهٔ پارامترهای US-A2 برای مقدار قبلی و جدید گزارش تغییرات (معیار پذیرش ۵).</summary>
    private static object Snapshot(SchemeSettings s) => new
    {
        ContractTypes = s.ContractTypes.ToList(), s.QardHasanRate, MurabahaRates = s.MurabahaRates.ToList(),
        RepaymentTermsMonths = s.RepaymentTermsMonths.ToList(), s.SalaryFromSepahMultiplierYes, s.SalaryFromSepahMultiplierNo,
        s.ContractDurationMonths, s.GraceMonthsAfterExpiry, s.MaxPlansPerCompany, s.PersonnelIntroductionDelayMonths,
        s.WaitingPeriodDays, s.DigitalApprovalEnabled,
        UsableTable = s.Plans.Where(p => p.IsActive).Select(p => new { p.TermMonths, p.ContractType, p.Rate, p.UsablePercent }).ToList(),
    };
}
