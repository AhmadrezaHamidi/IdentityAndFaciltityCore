namespace FacilityService.Tests;

/// <summary>قواعد خالص دامنه: تاریخ شمسی، عدد به حروف، بنر واجد شرایط بودن.</summary>
public class CreditRulesTests
{
    [Fact]
    public void Jalali_helpers_match_frontend()
    {
        Assert.Equal((1405, 7, 20), Jalali.Of(new DateOnly(2026, 10, 12)));
        Assert.Equal("1405/07", Jalali.MonthKey(new DateOnly(2026, 10, 12)));
        Assert.Equal(new DateOnly(2026, 7, 11), Jalali.AddMonths(new DateOnly(2026, 10, 12), -3));
        Assert.Equal(new DateOnly(2025, 3, 21), Jalali.ToDate(1404, 1, 1));
        Assert.Equal(["1405/11", "1405/12", "1406/01"], Jalali.ContractMonths(Jalali.ToDate(1405, 11, 5)).Take(3));
        Assert.Equal(2, Jalali.WholeMonthsBetween(Jalali.ToDate(1405, 4, 20), Jalali.ToDate(1405, 7, 19)));
    }

    [Fact]
    public void Persian_words_and_money_text_match_prototype()
    {
        Assert.Equal("یک میلیون و دویست و پنجاه هزار", PersianText.Words(1_250_000));
        Assert.Equal("صفر", PersianText.Words(0));
        Assert.Equal("۱۲,۳۴۵ ریال (۱,۲۳۵ تومان)", PersianText.Rial(12_345));
        Assert.Equal("پنج میلیون تومان", PersianText.TomanWords(50_000_000));
    }

    private static Company NewCompany(DateOnly contract, DateOnly opening)
    {
        var r = new Registrar(1, "u", null, null, "1");
        return Company.Create(1, "12345678901", "1", new CompanyData("شرکت تست", "NM-1", "12345678", contract, opening, false, [1]), 5, r, DateTime.UtcNow, null);
    }

    private static SchemeSettings Settings(int delay = 1)
    {
        var s = new SchemeSettings(1);
        s.Update(null, null, null, null, null, null, 12, 4, null, delay, null, null, null);
        return s;
    }

    [Fact]
    public void Banner_checks_introduction_delay_transfer_day_and_renewal()
    {
        var today = new DateOnly(2026, 10, 12); // ۱۴۰۵/۰۷/۲۰
        var company = NewCompany(Jalali.ToDate(1405, 7, 1), Jalali.ToDate(1405, 7, 20));
        Assert.Equal(EligibilityStatus.IntroductionNotYetAllowed, CreditRules.Banner(Settings(), company, today).Status);
        Assert.Equal(EligibilityStatus.Allowed, CreditRules.Banner(Settings(delay: 0), company, today).Status);

        var otherDay = NewCompany(Jalali.ToDate(1405, 1, 1), Jalali.ToDate(1405, 1, 3));
        var banner = CreditRules.Banner(Settings(), otherDay, today);
        Assert.Equal(EligibilityStatus.NotTransferDay, banner.Status);
        Assert.Equal(3, banner.AllowedDayOfMonth);

        // تمدید خودکار: قرارداد سال قبل که لغو نشده هنوز فعال است
        var old = NewCompany(Jalali.ToDate(1404, 2, 20), Jalali.ToDate(1404, 2, 20));
        Assert.Equal(EligibilityStatus.Allowed, CreditRules.Banner(Settings(), old, today).Status);
    }

    [Fact]
    public void Transfer_day_31_falls_on_the_last_day_of_shorter_months()
    {
        var company = NewCompany(Jalali.ToDate(1404, 1, 31), Jalali.ToDate(1404, 1, 31));
        Assert.Equal(EligibilityStatus.Allowed, CreditRules.Banner(Settings(), company, Jalali.ToDate(1405, 7, 30)).Status);
        Assert.Equal(EligibilityStatus.NotTransferDay, CreditRules.Banner(Settings(), company, Jalali.ToDate(1405, 7, 29)).Status);
        Assert.Equal(EligibilityStatus.Allowed, CreditRules.Banner(Settings(), company, Jalali.ToDate(1405, 2, 31)).Status);
        Assert.Equal(29, Jalali.TransferDayIn(company.AccountOpeningDate, Jalali.ToDate(1405, 12, 10)));
    }

    [Fact]
    public void Contract_year_renews_and_moves_the_balance_window()
    {
        var contract = Jalali.ToDate(1404, 3, 10);
        Assert.Equal(contract, Jalali.ContractYearStart(contract, Jalali.ToDate(1405, 3, 9)));
        Assert.Equal(Jalali.ToDate(1405, 3, 10), Jalali.ContractYearStart(contract, Jalali.ToDate(1405, 3, 10)));
        Assert.Equal(["1405/03", "1405/04"], Jalali.ContractMonths(contract, Jalali.ToDate(1405, 7, 1)).Take(2));
        Assert.Equal("1404/12", Jalali.PreviousMonthKey(Jalali.ToDate(1405, 1, 5)));
    }

    private static readonly Registrar By = new(1, "u", null, null, "1");

    private static (SchemeSettings, IReadOnlyList<SchemePlan>, Company) CreditCase()
    {
        var settings = Settings(delay: 0);
        settings.Update(null, null, null, null, 1, 1, null, null, null, null, null, null,
            [new UsablePercentRow(12, ContractType.QardHasan, 4, 50)]);
        var plans = settings.Plans.ToList();
        var company = NewCompany(Jalali.ToDate(1404, 1, 5), Jalali.ToDate(1404, 1, 5));
        var today = Jalali.ToDate(1405, 3, 5);
        // سال قبل قرارداد (۱۴۰۴/۰۱) و سال جاری (۱۴۰۵/۰۱ و ۰۲)
        company.SetBalances([("1404/01", 9_000_000_000)], By, DateTime.UtcNow, Jalali.ToDate(1404, 2, 1));
        company.SetBalances([("1405/01", 1_000_000_000), ("1405/02", 2_000_000_000)], By, DateTime.UtcNow, today);
        return (settings, plans, company);
    }

    [Fact]
    public void Credit_counts_only_the_current_contract_year()
    {
        var (settings, plans, company) = CreditCase();
        var today = Jalali.ToDate(1405, 3, 5);
        var lastYear = new Allocation(plans[0].Id, 400_000_000, Jalali.ToDate(1404, 6, 5));
        var credit = CreditRules.Calculate(settings, company, plans, [lastYear], today);
        Assert.Equal(3_000_000_000, credit.TotalAverageBase);           // میانگین ۱۴۰۴/۰۱ حساب نمی‌شود
        Assert.Equal(1_500_000_000, credit.Plans[0].CreatedCredit);      // ۵۰٪ × ۳ میلیارد
        Assert.Equal(0, credit.Plans[0].Allocated);                      // تخصیص سال قبل کم نمی‌شود
        Assert.False(credit.UsedLastMonth);
        Assert.Equal(1_500_000_000, credit.MonthRemainingCredit);
    }

    [Fact]
    public void Credit_is_capped_at_this_month_credit_after_using_last_month()
    {
        var (settings, plans, company) = CreditCase();
        var today = Jalali.ToDate(1405, 3, 5);
        var lastMonth = new Allocation(plans[0].Id, 200_000_000, Jalali.ToDate(1405, 2, 5));
        var thisMonth = new Allocation(plans[0].Id, 300_000_000, Jalali.ToDate(1405, 3, 5));
        var credit = CreditRules.Calculate(settings, company, plans, [lastMonth, thisMonth], today);
        Assert.True(credit.UsedLastMonth);
        // تجمعی: ۱٫۵ میلیارد − ۵۰۰ میلیون = ۱ میلیارد؛ سقف ماه جاری: ۵۰٪ × آخرین میانگین (۲ میلیارد) − ۳۰۰ میلیون = ۷۰۰ میلیون
        Assert.Equal(700_000_000, credit.Plans[0].RemainingThisMonth);

        var byMonth = CreditRules.CreditByMonth(settings, company, plans, [lastMonth, thisMonth], today);
        Assert.Equal(200_000_000, byMonth["1405/02"].Used);
        Assert.Equal(300_000_000, byMonth["1405/03"].Used);
    }

    [Fact]
    public void Preview_over_the_limit_suggests_a_fix_and_a_better_plan()
    {
        var (settings, plans, company) = CreditCase();
        var credit = CreditRules.Calculate(settings, company, plans, [], Jalali.ToDate(1405, 3, 5));
        var bigger = credit.Plans[0] with { RemainingThisMonth = 5_000_000_000 };
        var preview = CreditRules.Preview(100_000_000, AllocationMode.Single, 60_000_000, [null, null], [bigger]);
        Assert.False(preview.IsAllowed);
        Assert.Contains(preview.Messages, m => m.Level == HintLevel.Warning && m.Text.Contains("حداکثر مبلغ مجاز برای هر نفر"));
        Assert.Contains(preview.Messages, m => m.Level == HintLevel.Warning && m.Text.Contains("طرح مناسب‌تر"));

        var variable = CreditRules.Preview(100_000_000, AllocationMode.Variable, null, [80_000_000, 50_000_000]);
        Assert.Contains(variable.Messages, m => m.Level == HintLevel.Warning && m.Text.Contains("کم کنید"));
    }

    [Fact]
    public void Loan_status_transitions_follow_stages()
    {
        var by = new Registrar(1, "u", null, null, "1");
        var batch = new LoanBatch(1, "1", 1, AllocationMode.Single, new DateOnly(2026, 10, 12), null, by, DateTime.UtcNow);
        var r = batch.Add(1, 20_000_000, 1, by, DateTime.UtcNow);
        Assert.False(r.TryDecide(ApprovalStage.Credit, true, false, null, null, by, DateTime.UtcNow));
        Assert.True(r.TryDecide(ApprovalStage.Manager, true, false, null, null, by, DateTime.UtcNow));
        Assert.True(r.TryDecide(ApprovalStage.Credit, true, true, null, null, by, DateTime.UtcNow));
        Assert.Equal(LoanRequestStatus.PendingDigital, r.Status);
        Assert.Throws<FacilityDomainException>(() => r.RemoveFromBatch(by, DateTime.UtcNow));
        Assert.Throws<FacilityDomainException>(() => batch.Add(2, 5_000_000, 1, by, DateTime.UtcNow));
    }
}
