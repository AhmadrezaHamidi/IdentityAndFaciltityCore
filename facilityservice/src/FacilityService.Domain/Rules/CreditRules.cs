namespace FacilityService.Domain.Rules;

/// <summary>
/// قواعد اعتبار طرح — همان فرمول‌های نمونهٔ پنل (planAvailableCredit، eligibilityInfo، updateReqSuggestion):
///   اعتبار تجمعی هر طرح = درصد قابل استفاده × ضریب حقوق × مجموع میانگین‌های مثبت سال جاری قرارداد × سهم طرح (۱ ÷ تعداد طرح‌ها)
///   اعتبار ماه جاری = همان فرمول با آخرین میانگین مثبت
///   مانده = اعتبار تجمعی − تخصیص سال جاری قرارداد؛ اگر شرکت ماه قبل وام تخصیص داده، حداکثر اعتبار ماه جاری منهای تخصیص همین ماه
///   (سند: «اعتبار نمایش داده‌شده در ساجسشن باید با فرمول اعتبار ماه جاری (بر مبنای آخرین میانگین مثبت) همخوان باشد»).
/// قرارداد تا لغو نشده هر سال خودکار تمدید می‌شود؛ میانگین‌ها و تخصیص‌های سال‌های قبل در اعتبار سال جاری حساب نمی‌شوند.
/// </summary>
public static class CreditRules
{
    /// <param name="plans">طرح‌های تفاهم‌نامهٔ شرکت به ترتیب ثبت</param>
    /// <param name="allocations">درخواست‌های ردنشده/لغونشدهٔ شرکت (همهٔ طرح‌ها) با تاریخ مبنای ثبتشان</param>
    public static CompanyCredit Calculate(SchemeSettings settings, Company company, IReadOnlyList<SchemePlan> plans,
        IReadOnlyList<Allocation> allocations, DateOnly baseDate)
    {
        var multiplier = settings.MultiplierFor(company.SalaryFromSepah);
        var yearStart = Jalali.ContractYearStart(company.ContractDate, baseDate);
        var yearEnd = Jalali.AddMonths(yearStart, 12);
        var months = Jalali.ContractMonths(yearStart);
        var sumAverages = YearBalances(company, months).Where(b => b.Amount > 0).Sum(b => b.Amount);
        var lastAverage = LastAverage(company, months) ?? 0;
        var share = 1m / Math.Max(1, plans.Count);

        var thisMonth = Jalali.MonthKey(baseDate);
        var usedLastMonth = allocations.Any(a => Jalali.MonthKey(a.BaseDate) == Jalali.PreviousMonthKey(baseDate));
        var yearAllocations = allocations.Where(a => a.BaseDate >= yearStart && a.BaseDate < yearEnd).ToList();

        var rows = plans.Select(p =>
        {
            var percent = p.UsablePercent / 100m * multiplier * share;
            var created = Round(percent * sumAverages);
            var allocated = yearAllocations.Where(a => a.PlanId == p.Id).Sum(a => a.Amount);
            var remaining = Math.Max(0, created - allocated);
            if (usedLastMonth)
            {
                var allocatedThisMonth = yearAllocations.Where(a => a.PlanId == p.Id && Jalali.MonthKey(a.BaseDate) == thisMonth).Sum(a => a.Amount);
                remaining = Math.Min(remaining, Math.Max(0, Round(percent * lastAverage) - allocatedThisMonth));
            }
            return new PlanCredit(p, Round(sumAverages * share), created, allocated, remaining);
        }).ToList();

        return new CompanyCredit(baseDate, Banner(settings, company, baseDate), rows.Sum(r => r.RemainingThisMonth), sumAverages,
            LastAverage(company, months), rows, usedLastMonth, yearStart);
    }

    /// <summary>بنر «هشدارهای وضعیت واجد شرایط بودن» بالای تب تخصیص وام.</summary>
    public static EligibilityBanner Banner(SchemeSettings settings, Company company, DateOnly baseDate)
    {
        if (company.Status != CompanyStatus.Active || company.IsDeleted)
            return new(EligibilityStatus.ContractInactive, "قرارداد این شرکت فعال نیست.", null);

        if (Jalali.WholeMonthsBetween(company.ContractDate, baseDate) < settings.PersonnelIntroductionDelayMonths)
            return new(EligibilityStatus.IntroductionNotYetAllowed,
                "شرکت هنوز به مهلت معرفی پرسنل نرسیده و مجاز به معرفی پرسنل برای وام نیست.", null);

        var day = Jalali.TransferDayIn(company.AccountOpeningDate, baseDate);
        if (Jalali.DayOf(baseDate) != day)
            return new(EligibilityStatus.NotTransferDay,
                $"امروز روز مجاز انتقال اعتبار نیست. انتقال فقط در روز «{PersianText.Number(Jalali.DayOf(company.AccountOpeningDate))}» هر ماه (مطابق روز افتتاح حساب سپرده) مجاز است.", day);

        return new(EligibilityStatus.Allowed, "شرکت مجاز به ثبت درخواست وام در این ماه است.", day);
    }

    /// <summary>آخرین میانگین مثبت در ماه‌های سال جاری قرارداد (در تاریخ on).</summary>
    public static long? LastAverage(Company company, DateOnly on) => LastAverage(company, Jalali.ContractMonths(company.ContractDate, on));

    private static long? LastAverage(Company company, IReadOnlyList<string> months)
    {
        long? last = null;
        foreach (var month in months)
        {
            var row = company.Balances.FirstOrDefault(b => b.Month == month);
            if (row is { Amount: > 0 }) last = row.Amount;
        }
        return last;
    }

    private static IEnumerable<MonthlyBalance> YearBalances(Company company, IReadOnlyList<string> months) =>
        company.Balances.Where(b => months.Contains(b.Month));

    /// <summary>
    /// ستون‌های جدول میانگین‌ها برای هر ماه سال جاری قرارداد: «مانده اعتبار تا این ماه» (اعتبار تجمعی تا آن ماه منهای تخصیص سال)
    /// و «اعتبار مصرف‌شده» (تخصیص‌هایی که تاریخ مبنایشان در همان ماه است).
    /// </summary>
    public static IReadOnlyDictionary<string, (long Remaining, long Used)> CreditByMonth(SchemeSettings settings, Company company,
        IReadOnlyList<SchemePlan> plans, IReadOnlyList<Allocation> allocations, DateOnly on)
    {
        var multiplier = settings.MultiplierFor(company.SalaryFromSepah);
        var share = 1m / Math.Max(1, plans.Count);
        var percentSum = plans.Sum(p => p.UsablePercent / 100m);
        var yearStart = Jalali.ContractYearStart(company.ContractDate, on);
        var yearEnd = Jalali.AddMonths(yearStart, 12);
        var yearAllocations = allocations.Where(a => a.BaseDate >= yearStart && a.BaseDate < yearEnd).ToList();
        var totalAllocated = yearAllocations.Sum(a => a.Amount);
        var running = 0L;
        var result = new Dictionary<string, (long, long)>();
        foreach (var month in Jalali.ContractMonths(yearStart))
        {
            running += Math.Max(0, company.Balances.FirstOrDefault(b => b.Month == month)?.Amount ?? 0);
            var used = yearAllocations.Where(a => Jalali.MonthKey(a.BaseDate) == month).Sum(a => a.Amount);
            result[month] = (Math.Max(0, Round(percentSum * multiplier * running * share) - totalAllocated), used);
        }
        return result;
    }

    /// <summary>هینت‌های زندهٔ فرم تخصیص (همان updateReqSuggestion نمونه).</summary>
    /// <param name="amounts">در حالت متغیر: مبلغ هر نفر (null = هنوز وارد نشده)</param>
    /// <param name="otherPlans">طرح‌های دیگر تفاهم‌نامه برای پیشنهاد «طرح مناسب‌تر» وقتی مبلغ از اعتبار این طرح بیشتر است</param>
    public static LoanPreview Preview(long remaining, AllocationMode mode, long? amount, IReadOnlyList<long?> amounts,
        IReadOnlyList<PlanCredit>? otherPlans = null)
    {
        var count = amounts.Count;
        var msgs = new List<Hint>();
        LoanPreview Result(long total, bool allowed, int? max = null, long? suggested = null) =>
            new(count, total, remaining, allowed, max, suggested, msgs);
        void SuggestPlan(long needed)
        {
            var better = otherPlans?.Where(p => p.RemainingThisMonth >= needed).OrderByDescending(p => p.RemainingThisMonth).FirstOrDefault();
            if (better != null)
                msgs.Add(new(HintLevel.Warning, $"طرح مناسب‌تر: «{better.Plan.Title}» با اعتبار باقی‌ماندهٔ {PersianText.Rial(better.RemainingThisMonth)} این مبلغ را پوشش می‌دهد."));
        }
        var available = $"اعتبار قابل استفاده این طرح در این ماه: {PersianText.Rial(remaining)} ({PersianText.TomanWords(remaining)})";

        if (mode == AllocationMode.Variable)
        {
            var sum = amounts.Sum(a => a ?? 0);
            if (count == 0)
            {
                msgs.Add(new(HintLevel.Info, available));
                return Result(0, false);
            }
            if (amounts.Any(a => a is null or 0))
            {
                msgs.Add(new(HintLevel.Info, "برای همه پرسنل انتخاب‌شده، مبلغ وام را در جدول بالا وارد کنید."));
                return Result(sum, false);
            }
            if (sum > remaining)
            {
                msgs.Add(new(HintLevel.Error, $"مجموع مبالغ وارد‌شده ({PersianText.Rial(sum)}) بیشتر از اعتبار باقی‌مانده این طرح ({PersianText.Rial(remaining)}) است."));
                msgs.Add(new(HintLevel.Warning, $"پیشنهاد: مجموع مبالغ را حداقل {PersianText.Rial(sum - remaining)} کم کنید تا به سقف {PersianText.Rial(remaining)} برسد."));
                SuggestPlan(sum);
                return Result(sum, false);
            }
            msgs.Add(new(HintLevel.Success, $"مجموع مبالغ {PersianText.Number(count)} نفر: {PersianText.Rial(sum)} ({PersianText.TomanWords(sum)}) — مجاز است."));
            return Result(sum, true);
        }

        if (amount is null or 0)
        {
            msgs.Add(new(HintLevel.Info, available));
            return Result(0, false);
        }
        var max = (int)Math.Min(int.MaxValue, remaining / amount.Value);
        var total = amount.Value * count;
        if (count == 0)
        {
            msgs.Add(new(HintLevel.Info, $"با این مبلغ برای هر نفر ({PersianText.TomanWords(amount.Value)})، حداکثر می‌توانید به {PersianText.Number(max)} نفر با این طرح وام بدهید. لطفاً پرسنل را انتخاب کنید."));
            return Result(0, false, max);
        }
        if (total > remaining)
        {
            msgs.Add(new(HintLevel.Error, $"مجموع مبلغ برای {PersianText.Number(count)} نفر انتخاب‌شده ({PersianText.Rial(total)}) بیشتر از اعتبار باقی‌مانده این طرح است. حداکثر تعداد نفراتی که با این مبلغ می‌توان پوشش داد: {PersianText.Number(max)} نفر."));
            msgs.Add(new(HintLevel.Warning, max > 0
                ? $"پیشنهاد: حداکثر مبلغ مجاز برای هر نفر {PersianText.Rial(remaining / count)} است، یا تعداد نفرات را به {PersianText.Number(max)} نفر کاهش دهید."
                : $"پیشنهاد: حداکثر مبلغ مجاز برای هر نفر {PersianText.Rial(remaining / count)} است."));
            SuggestPlan(total);
            return Result(total, false, max, remaining / count);
        }
        msgs.Add(new(HintLevel.Success, $"مجموع مبلغ برای {PersianText.Number(count)} نفر: {PersianText.Rial(total)} ({PersianText.TomanWords(total)}) — مجاز است. با اعتبار باقی‌مانده، حداکثر تا {PersianText.Number(max)} نفر با این مبلغ قابل پوشش است."));
        return Result(total, true, max);
    }

    private static long Round(decimal v) => (long)Math.Round(v, MidpointRounding.AwayFromZero);
}
