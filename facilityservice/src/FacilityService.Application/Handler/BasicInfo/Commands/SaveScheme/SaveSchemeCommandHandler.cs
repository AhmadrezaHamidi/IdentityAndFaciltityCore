using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Commands.SaveScheme;

public sealed class SaveSchemeCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : SchemeHandlerBase(db, user, time), IRequestHandler<SaveSchemeCommand, FacilityBaseMessageResponse<SchemeFormDto>>
{
    public async Task<FacilityBaseMessageResponse<SchemeFormDto>> Handle(SaveSchemeCommand request, CancellationToken ct)
    {
        var creating = request.SchemeId <= 0;
        var (scheme, settings) = creating ? await CreateAsync(request, ct) : await LoadAsync(request, ct);

        var before = creating ? null : Snapshot(scheme, settings);

        scheme.Rename(request.Title);
        scheme.SetActive(request.IsActive);
        settings.Update(
            request.ContractTypes, request.QardHasanRate, request.MurabahaRates, request.RepaymentTermsMonths,
            request.SalaryFromSepahMultiplierYes, request.SalaryFromSepahMultiplierNo, request.ContractDurationMonths,
            request.GraceMonthsAfterExpiry, request.MaxPlansPerCompany, request.PersonnelIntroductionDelayMonths,
            request.WaitingPeriodDays, request.DigitalApprovalEnabled,
            request.UsableTable?.Select(u => new UsablePercentRow(u.TermMonths, u.ContractType, u.Rate, u.UsablePercent)).ToList());

        await MergeReasonsAsync(scheme.Id, ReasonType.ContractCancel, request.ContractCancelReasons, ct);
        await MergeReasonsAsync(scheme.Id, ReasonType.LoanReject, request.LoanRejectReasons, ct);
        await MergeReasonsAsync(scheme.Id, ReasonType.RemoteGrantDenied, request.RemoteGrantDeniedReasons, ct);
        await MergeReasonsAsync(scheme.Id, ReasonType.DocumentType, request.DocumentTypes, ct);
        await MergeReasonsAsync(scheme.Id, ReasonType.LoanCancel, request.LoanCancelReasons, ct);
        await MergeReasonsAsync(scheme.Id, ReasonType.PersonnelReject, request.PersonnelRejectReasons, ct);

        Log(ChangeLogCategory.Company, creating ? "ایجاد طرح" : "ویرایش طرح", $"طرح {scheme.Title}", null,
            before, Snapshot(scheme, settings));
        await Db.SaveChangesAsync(ct);
        return Ok(await FormAsync(scheme, ct), creating ? "طرح ثبت شد." : "طرح ویرایش شد.");
    }

    private async Task<(Scheme, SchemeSettings)> CreateAsync(SaveSchemeCommand request, CancellationToken ct)
    {
        var code = (request.Code ?? "").Trim();
        if (code.Length == 0) throw Invalid("کد طرح الزامی است.");
        if (await Db.Schemes.AnyAsync(s => s.Code == code, ct)) throw Invalid("طرحی با این کد وجود دارد.");

        // طرح اول ذخیره می‌شود تا Id بگیرد؛ تنظیمات و دلایل به آن Id وصل‌اند.
        var scheme = new Scheme(code, request.Title);
        Db.Schemes.Add(scheme);
        await Db.SaveChangesAsync(ct);

        var settings = new SchemeSettings(scheme.Id);
        Db.SchemeSettings.Add(settings);
        return (scheme, settings);
    }

    private async Task<(Scheme, SchemeSettings)> LoadAsync(SaveSchemeCommand request, CancellationToken ct)
    {
        var scheme = await Db.Schemes.FirstOrDefaultAsync(s => s.Id == request.SchemeId, ct)
                     ?? throw FacilityDomainException.NotFound("طرح یافت نشد.");
        var settings = await SettingsAsync(scheme.Id, ct);
        if (string.IsNullOrWhiteSpace(request.RowVersion)) throw Invalid("نسخهٔ رکورد (rowVersion) الزامی است.");
        settings.EnsureRowVersion(request.RowVersion);
        return (scheme, settings);
    }

    /// <summary>همهٔ موارد فرم برای مقدار قبلی و جدید گزارش تغییرات (معیار پذیرش ۵).</summary>
    private static object Snapshot(Scheme scheme, SchemeSettings s) => new
    {
        scheme.Code, scheme.Title, scheme.IsActive,
        ContractTypes = s.ContractTypes.ToList(), s.QardHasanRate, MurabahaRates = s.MurabahaRates.ToList(),
        RepaymentTermsMonths = s.RepaymentTermsMonths.ToList(), s.SalaryFromSepahMultiplierYes, s.SalaryFromSepahMultiplierNo,
        s.ContractDurationMonths, s.GraceMonthsAfterExpiry, s.MaxPlansPerCompany, s.PersonnelIntroductionDelayMonths,
        s.WaitingPeriodDays, s.DigitalApprovalEnabled,
        UsableTable = s.Plans.Where(p => p.IsActive).Select(p => new { p.TermMonths, p.ContractType, p.Rate, p.UsablePercent }).ToList(),
    };
}
