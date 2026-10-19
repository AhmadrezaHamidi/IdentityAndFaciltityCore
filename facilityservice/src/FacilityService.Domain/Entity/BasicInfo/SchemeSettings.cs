namespace FacilityService.Domain.Entity.BasicInfo;

/// <summary>
/// تنظیمات اطلاعات پایهٔ هر طرح (US-A2). نرخ‌ها و درصدها به «درصد» هستند (۴ یعنی ۴٪) — مثل نمونهٔ پنل و فرانت.
/// </summary>
public class SchemeSettings : VersionedEntity
{
    public int SchemeId { get; private set; }
    public List<ContractType> ContractTypes { get; private set; } = [];
    public decimal QardHasanRate { get; private set; }
    public List<decimal> MurabahaRates { get; private set; } = [];
    public List<int> RepaymentTermsMonths { get; private set; } = [];
    public decimal SalaryFromSepahMultiplierYes { get; private set; } = 1;
    public decimal SalaryFromSepahMultiplierNo { get; private set; } = 1;
    public int ContractDurationMonths { get; private set; } = 12;
    public int GraceMonthsAfterExpiry { get; private set; }
    public int MaxPlansPerCompany { get; private set; } = 1;
    public int PersonnelIntroductionDelayMonths { get; private set; }
    public int WaitingPeriodDays { get; private set; }

    /// <summary>مرحلهٔ «تأیید شعبهٔ دیجیتال» بعد از تأیید اعتبارات ⚠ Q: سند و نمونه این مرحله را اختیاری گذاشته‌اند.</summary>
    public bool DigitalApprovalEnabled { get; private set; }

    public List<SchemePlan> Plans { get; private set; } = [];

    private SchemeSettings() { }

    public SchemeSettings(int schemeId) => SchemeId = schemeId;

    /// <summary>ذخیرهٔ تنظیمات. مقدار null یعنی «تغییر نکند» (پر کردن همهٔ موارد الزامی نیست).</summary>
    public void Update(
        IReadOnlyList<ContractType>? contractTypes, decimal? qardHasanRate, IReadOnlyList<decimal>? murabahaRates,
        IReadOnlyList<int>? repaymentTerms, decimal? multiplierYes, decimal? multiplierNo, int? contractDurationMonths,
        int? graceMonths, int? maxPlans, int? introductionDelayMonths, int? waitingPeriodDays, bool? digitalApprovalEnabled,
        IReadOnlyList<UsablePercentRow>? usableTable)
    {
        if (contractTypes != null)
        {
            if (contractTypes.Count is < 1 or > 2 || contractTypes.Distinct().Count() != contractTypes.Count || contractTypes.Any(t => !Enum.IsDefined(t)))
                throw new FacilityDomainException("نوع عقد: حداقل یک و حداکثر دو مورد (قرض‌الحسنه، مرابحه).");
            ContractTypes = contractTypes.ToList();
        }
        if (qardHasanRate != null)
        {
            if (qardHasanRate is < 0 or > 4) throw new FacilityDomainException("نرخ قرض‌الحسنه باید بین ۰ تا ۴ درصد باشد.");
            QardHasanRate = qardHasanRate.Value;
            // مثل نمونهٔ پنل: نرخ همهٔ طرح‌های قرض‌الحسنه همین یک نرخ است
            foreach (var plan in Plans.Where(p => p.ContractType == ContractType.QardHasan)) plan.ChangeRate(QardHasanRate);
        }
        if (murabahaRates != null)
        {
            if (murabahaRates.Count > 5 || murabahaRates.Any(r => r is < 0 or > 23))
                throw new FacilityDomainException("نرخ‌های مرابحه: حداکثر ۵ نرخ، هرکدام ۰ تا ۲۳ درصد.");
            MurabahaRates = murabahaRates.Distinct().OrderBy(r => r).ToList();
        }
        if (repaymentTerms != null)
        {
            if (repaymentTerms.Any(t => t % 3 != 0 || t is < 3 or > 60))
                throw new FacilityDomainException("مدت‌های بازپرداخت باید مضرب ۳ و بین ۳ تا ۶۰ ماه باشد.");
            RepaymentTermsMonths = repaymentTerms.Distinct().OrderBy(t => t).ToList();
        }
        if (multiplierYes != null) SalaryFromSepahMultiplierYes = Multiplier(multiplierYes.Value);
        if (multiplierNo != null) SalaryFromSepahMultiplierNo = Multiplier(multiplierNo.Value);
        if (contractDurationMonths != null) ContractDurationMonths = Range(contractDurationMonths.Value, 1, 120, "مدت قرارداد: ۱ تا ۱۲۰ ماه.");
        if (graceMonths != null) GraceMonthsAfterExpiry = Range(graceMonths.Value, 0, 120, "مهلت استفاده پس از انقضا: ۰ تا ۱۲۰ ماه.");
        if (maxPlans != null) MaxPlansPerCompany = Range(maxPlans.Value, 1, 10, "حداکثر تعداد طرح: ۱ تا ۱۰.");
        if (introductionDelayMonths != null) PersonnelIntroductionDelayMonths = Range(introductionDelayMonths.Value, 0, 120, "مهلت معرفی پرسنل: ۰ تا ۱۲۰ ماه.");
        if (waitingPeriodDays != null) WaitingPeriodDays = Range(waitingPeriodDays.Value, 0, 180, "دورهٔ انتظار: ۰ تا ۱۸۰ روز.");
        if (digitalApprovalEnabled != null) DigitalApprovalEnabled = digitalApprovalEnabled.Value;
        if (usableTable != null) ReplacePlans(usableTable);
        Bump();
    }

    /// <summary>
    /// طرح‌ها با (نوع عقد، نرخ، مدت) شناخته می‌شوند تا Id طرح‌هایی که شرکت‌ها و درخواست‌ها دارند ثابت بماند.
    /// ردیف حذف‌شده غیرفعال می‌شود، نه پاک.
    /// </summary>
    private void ReplacePlans(IReadOnlyList<UsablePercentRow> rows)
    {
        foreach (var r in rows)
        {
            if (r.UsablePercent is < 0.1m or > 100) throw new FacilityDomainException("درصد قابل استفاده باید بین ۰٫۱ تا ۱۰۰ باشد.");
            if (r.TermMonths % 3 != 0 || r.TermMonths is < 3 or > 60) throw new FacilityDomainException("مدت طرح باید مضرب ۳ و بین ۳ تا ۶۰ ماه باشد.");
            if (r.Rate is < 0 or > 23) throw new FacilityDomainException("نرخ طرح باید بین ۰ تا ۲۳ درصد باشد.");
        }
        // ردیف قرض‌الحسنه نرخ جدا ندارد؛ همیشه نرخ ثابت اطلاعات پایه (نمونهٔ پنل: usable[term].qarz)
        rows = rows.Select(r => r.ContractType == ContractType.QardHasan ? r with { Rate = QardHasanRate } : r).ToList();
        if (rows.GroupBy(r => (r.ContractType, r.Rate, r.TermMonths)).Any(g => g.Count() > 1))
            throw new FacilityDomainException("در جدول درصد قابل استفاده ردیف تکراری وجود دارد.");

        foreach (var plan in Plans) plan.Deactivate();
        foreach (var r in rows)
        {
            var plan = Plans.FirstOrDefault(p => p.Matches(r.ContractType, r.Rate, r.TermMonths));
            if (plan == null) Plans.Add(new SchemePlan(SchemeId, r.ContractType, r.Rate, r.TermMonths, r.UsablePercent));
            else plan.Activate(r.UsablePercent);
        }
    }

    private static decimal Multiplier(decimal v) =>
        v is >= 0.01m and <= 10 ? Math.Round(v, 2) : throw new FacilityDomainException("ضریب حقوق باید بین ۰٫۰۱ تا ۱۰ باشد.");

    private static int Range(int v, int min, int max, string message) =>
        v >= min && v <= max ? v : throw new FacilityDomainException(message);

    public decimal MultiplierFor(bool salaryFromSepah) => salaryFromSepah ? SalaryFromSepahMultiplierYes : SalaryFromSepahMultiplierNo;
}
