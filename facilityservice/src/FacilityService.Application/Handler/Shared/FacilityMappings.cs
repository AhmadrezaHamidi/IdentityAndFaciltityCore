
namespace FacilityService.Application.Handler.Shared;

/// <summary>تبدیل موجودیت‌ها به DTOهای قرارداد پنل.</summary>
public static class FacilityMappings
{
    public static RegistrarDto ToDto(this Registrar r) => new(r.PanelUserId, r.UserName, r.UserCode, r.BranchName, r.BranchCode);

    public static StoredFileDto ToDto(this StoredFile f) => new(f.Id, f.FileName, f.Size, f.UploadedAt, f.UploadedBy);

    public static PlanDto ToDto(this SchemePlan p) => new(p.Id, p.ContractType, p.Rate, p.TermMonths, p.UsablePercent, p.Title);

    public static BasicInfoDto ToDto(this SchemeSettings s) => new(
        s.SchemeId, s.ContractTypes, s.QardHasanRate, s.MurabahaRates, s.RepaymentTermsMonths,
        s.SalaryFromSepahMultiplierYes, s.SalaryFromSepahMultiplierNo, s.ContractDurationMonths, s.GraceMonthsAfterExpiry,
        s.MaxPlansPerCompany, s.PersonnelIntroductionDelayMonths, s.WaitingPeriodDays,
        s.Plans.Where(p => p.IsActive).OrderBy(p => p.ContractType).ThenBy(p => p.TermMonths).ThenBy(p => p.Rate)
            .Select(p => new UsablePercentDto(p.TermMonths, p.ContractType, p.Rate, p.UsablePercent)).ToList(),
        s.RowVersion, s.DigitalApprovalEnabled);

    public static ReasonDto ToDto(this Reason r) => new(r.Id, r.Type, r.Title, r.IsActive, r.SortOrder);

    public static CompanyListItemDto ToListItem(this Company c) => new(
        c.Id, c.Name, c.NationalId, c.AgreementNumber, c.ContractDate, c.Plans.Count, c.Registrar.ToDto(), c.Status, c.BranchCode,
        // فرانت در ستون وضعیت «لغوشده در تاریخ … (دلیل)» را نشان می‌دهد و تا امروز ناچار بود
        // برای هر ردیف جزئیات شرکت را جدا بگیرد.
        c.Cancellation?.CancelDate, c.Cancellation?.ReasonTitle);

    public static CompanyDetailsDto ToDetails(this Company c, IReadOnlyList<SchemePlan> allPlans) => new(
        c.Id, c.SchemeId, c.Name, c.NationalId, c.AgreementNumber, c.AccountNumber, c.ContractDate, c.AccountOpeningDate,
        Jalali.DayOf(c.AccountOpeningDate), c.SalaryFromSepah,
        c.Plans.Select(cp => new PlanRefDto(cp.PlanId, allPlans.FirstOrDefault(p => p.Id == cp.PlanId)?.Title ?? cp.PlanId.ToString())).ToList(),
        c.AgreementFile?.ToDto(), c.Registrar.ToDto(), c.CreatedAt, c.Status,
        c.Cancellation is { } x
            ? new CancellationInfoDto(x.CancelDate, x.ReasonTitle, x.Description, x.LetterFile?.ToDto(), x.CancelledBy.ToDto(), x.CancelledAt)
            : null,
        c.BranchCode, c.RowVersion);

    public static PersonnelListItemDto ToListItem(this Personnel p, bool isLocked) => new(
        p.Id, p.Priority, p.FullName, p.NationalCode, p.Mobile, p.PersonnelCode, p.HireDate, p.Status, p.RejectReason,
        p.CreatedAt, p.DecidedAt, p.Registrar.ToDto(), isLocked);

    public static PersonnelDetailsDto ToDetails(this Personnel p, bool isLocked) => new(
        p.Id, p.CompanyId, p.Priority, p.FullName, p.NationalCode, p.Mobile, p.PersonnelCode, p.HireDate, p.Status, isLocked,
        p.History.OrderBy(h => h.At).ThenBy(h => h.Id)
            .Select(h => new PersonnelHistoryDto(h.From, h.To, h.Reason, h.Description, h.By.ToDto(), h.At)).ToList(),
        p.RowVersion);
}
