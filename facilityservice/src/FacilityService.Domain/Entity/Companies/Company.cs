using System.Text.RegularExpressions;

namespace FacilityService.Domain.Entity.Companies;

/// <summary>شرکت طرف تفاهم‌نامه. دامنهٔ دسترسی: BranchCode (کاربر شعبه) و Id (مدیر و منابع انسانی شرکت).</summary>
public class Company : VersionedEntity
{
    public long Id { get; private set; }
    public int SchemeId { get; private set; }
    public string Name { get; private set; } = null!;
    public string NationalId { get; private set; } = null!;
    public string AgreementNumber { get; private set; } = null!;
    public string AccountNumber { get; private set; } = null!;
    public DateOnly ContractDate { get; private set; }
    public DateOnly AccountOpeningDate { get; private set; }
    public bool SalaryFromSepah { get; private set; }
    public string BranchCode { get; private set; } = null!;
    public CompanyStatus Status { get; private set; } = CompanyStatus.Active;
    public Registrar Registrar { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public long? AgreementFileId { get; private set; }
    public StoredFile? AgreementFile { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeleteDescription { get; private set; }

    /// <summary>نسخهٔ جدا برای جدول میانگین‌ها تا ویرایش فرم شرکت و ثبت میانگین هم‌زمان با هم تداخل نکنند.</summary>
    public int BalancesVersion { get; private set; } = 1;

    public CompanyCancellation? Cancellation { get; private set; }
    public List<CompanyPlan> Plans { get; private set; } = [];
    public List<MonthlyBalance> Balances { get; private set; } = [];

    private Company() { }

    public static readonly DateOnly MinContractDate = Jalali.ToDate(1400, 1, 1);
    public static readonly DateOnly MaxContractDate = Jalali.ToDate(1450, 12, 29);
    public static readonly DateOnly MinAccountOpeningDate = Jalali.ToDate(1380, 1, 1);

    public static Company Create(int schemeId, string nationalId, string branchCode, CompanyData data, int maxPlans,
        Registrar by, DateTime now, StoredFile? agreementFile)
    {
        if (!Regex.IsMatch(nationalId ?? "", @"^\d{11}$")) throw new FacilityDomainException("شناسهٔ ملی شرکت باید ۱۱ رقم باشد.");
        if (string.IsNullOrWhiteSpace(branchCode)) throw new FacilityDomainException("کد شعبهٔ شرکت مشخص نیست.");
        var c = new Company
        {
            SchemeId = schemeId,
            NationalId = nationalId!,
            BranchCode = branchCode.Trim(),
            Registrar = by,
            CreatedAt = now,
            AgreementFile = agreementFile,
        };
        c.Apply(data, maxPlans);
        return c;
    }

    public bool IsDeleted => DeletedAt != null;

    public void Update(CompanyData data, int maxPlans, StoredFile? newAgreementFile)
    {
        EnsureNotDeleted();
        Apply(data, maxPlans);
        if (newAgreementFile != null) AgreementFile = newAgreementFile;
        Bump();
    }

    private void Apply(CompanyData d, int maxPlans)
    {
        var name = (d.Name ?? "").Trim();
        if (!Regex.IsMatch(name, @"^[؀-ۿ‌\s0-9۰-۹]{3,100}$")) throw new FacilityDomainException("نام شرکت: ۳ تا ۱۰۰ حرف فارسی، فاصله و عدد.");
        if (!Regex.IsMatch(d.AgreementNumber ?? "", @"^[\p{L}0-9/\-.]{3,50}$")) throw new FacilityDomainException("شمارهٔ تفاهم‌نامه: ۳ تا ۵۰ حرف، عدد، / - .");
        if (!Regex.IsMatch(d.AccountNumber ?? "", @"^\d{8,24}$")) throw new FacilityDomainException("شمارهٔ حساب باید ۸ تا ۲۴ رقم باشد.");
        if (d.ContractDate < MinContractDate || d.ContractDate > MaxContractDate) throw new FacilityDomainException("تاریخ انعقاد قرارداد باید بین ۱۴۰۰/۰۱/۰۱ و ۱۴۵۰/۱۲/۲۹ باشد.");
        if (d.AccountOpeningDate < MinAccountOpeningDate || d.AccountOpeningDate > MaxContractDate)
            throw new FacilityDomainException("تاریخ افتتاح حساب سپرده باید بین ۱۳۸۰/۰۱/۰۱ و ۱۴۵۰/۱۲/۲۹ باشد.");
        var planIds = (d.PlanIds ?? []).Distinct().ToList();
        if (planIds.Count == 0) throw new FacilityDomainException("حداقل یک طرح باید انتخاب شود.");
        if (planIds.Count > maxPlans) throw new FacilityDomainException($"حداکثر {PersianText.Number(maxPlans)} طرح قابل انتخاب است.");

        Name = name;
        AgreementNumber = d.AgreementNumber!;
        AccountNumber = d.AccountNumber!;
        ContractDate = d.ContractDate;
        AccountOpeningDate = d.AccountOpeningDate;
        SalaryFromSepah = d.SalaryFromSepah;
        Plans.RemoveAll(p => !planIds.Contains(p.PlanId));
        foreach (var id in planIds.Where(id => Plans.All(p => p.PlanId != id)))
            Plans.Add(new CompanyPlan(id));
    }

    /// <summary>US-B6: لغو قرارداد. تاریخ از ۱۴۰۵/۰۱/۰۱ تا امروز؛ دلیل، توضیح و فایل نامه الزامی.</summary>
    public void Cancel(DateOnly cancelDate, Reason reason, string description, StoredFile letterFile, Registrar by, DateTime now, DateOnly today)
    {
        EnsureNotDeleted();
        if (Status != CompanyStatus.Active) throw new FacilityDomainException("قرارداد این شرکت فعال نیست.");
        if (reason.Type != ReasonType.ContractCancel || !reason.IsActive) throw new FacilityDomainException("انتخاب دلیل لغو الزامی است.");
        if (cancelDate < Jalali.ToDate(1405, 1, 1) || cancelDate > today) throw new FacilityDomainException("تاریخ لغو باید از ۱۴۰۵/۰۱/۰۱ تا امروز باشد.");
        description = (description ?? "").Trim();
        if (description.Length is < 5 or > 1000) throw new FacilityDomainException("درج دلیل (۵ تا ۱۰۰۰ حرف) الزامی است.");
        Status = CompanyStatus.Cancelled;
        Cancellation = new CompanyCancellation(cancelDate, reason.Id, reason.Title, description, letterFile, by, now);
        Bump();
    }

    /// <summary>حذف نرم؛ در داشبورد «شرکت‌های حذف‌شده» می‌ماند. شرکتی که درخواست تأییدشده دارد حذف نمی‌شود (بررسی در Handler).</summary>
    public void Delete(string description, DateTime now)
    {
        EnsureNotDeleted();
        description = (description ?? "").Trim();
        if (description.Length is < 5 or > 500) throw new FacilityDomainException("توضیح حذف: ۵ تا ۵۰۰ حرف.");
        DeletedAt = now;
        DeleteDescription = description;
        Status = CompanyStatus.Deleted;
        Bump();
    }

    /// <summary>US-B4: ثبت میانگین موجودی ماه‌های سال جاری قرارداد (در تاریخ today). null یعنی خالی کردن ماه.</summary>
    /// <returns>ماه‌هایی که واقعاً عوض شدند با مقدار قبلی و جدید (برای گزارش تغییرات).</returns>
    public IReadOnlyList<(string Month, long? Before, long? After)> SetBalances(
        IReadOnlyList<(string Month, long? Amount)> months, Registrar by, DateTime now, DateOnly today)
    {
        EnsureNotDeleted();
        var valid = Jalali.ContractMonths(ContractDate, today);
        foreach (var (month, amount) in months)
        {
            if (!valid.Contains(month)) throw new FacilityDomainException($"ماه {PersianText.Digits(month)} در سال جاری قرارداد نیست.");
            if (amount is < 1 or > 999_999_999_999_999) throw new FacilityDomainException("میانگین موجودی باید بین ۱ ریال و ۱۵ رقم باشد.");
        }
        var changes = new List<(string, long?, long?)>();
        foreach (var (month, amount) in months)
        {
            var row = Balances.FirstOrDefault(b => b.Month == month);
            var before = row?.Amount;
            if (before == amount) continue;
            if (amount == null) Balances.Remove(row!);
            else if (row == null) Balances.Add(new MonthlyBalance(month, amount.Value, by, now));
            else row.Set(amount.Value, by, now);
            changes.Add((month, before, amount));
        }
        BalancesVersion++;
        return changes;
    }

    /// <summary>بازگردانی میانگین‌ها از گزارش تغییرات.</summary>
    public void RestoreBalances(IReadOnlyDictionary<string, long> snapshot, Registrar by, DateTime now)
    {
        Balances.RemoveAll(b => !snapshot.ContainsKey(b.Month));
        foreach (var (month, amount) in snapshot)
        {
            var row = Balances.FirstOrDefault(b => b.Month == month);
            if (row == null) Balances.Add(new MonthlyBalance(month, amount, by, now));
            else if (row.Amount != amount) row.Set(amount, by, now);
        }
        BalancesVersion++;
    }

    public void EnsureBalancesVersion(string? rowVersion)
    {
        if (rowVersion != BalancesVersion.ToString()) throw FacilityDomainException.Conflict("میانگین‌ها هم‌زمان توسط کاربر دیگری تغییر کرده است.");
    }

    /// <summary>
    /// تخصیص اعتبار: نسخهٔ شرکت بالا می‌رود تا دو ثبت هم‌زمان روی یک شرکت با هم از اعتبار طرح بیشتر برندارند
    /// (دومی هنگام ذخیره Conflict می‌گیرد و با اعتبار تازه دوباره بررسی می‌شود).
    /// </summary>
    public void ReserveCredit() => Bump();

    public IReadOnlyDictionary<string, long> BalanceSnapshot() => Balances.ToDictionary(b => b.Month, b => b.Amount);

    private void EnsureNotDeleted()
    {
        if (IsDeleted) throw FacilityDomainException.NotFound("شرکت یافت نشد.");
    }
}
