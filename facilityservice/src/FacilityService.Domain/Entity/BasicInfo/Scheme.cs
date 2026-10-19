namespace FacilityService.Domain.Entity.BasicInfo;

/// <summary>طرح کلان (نگین میثاق، شجره طیبه، هم‌پیمان، ...) — فیلد ۲۰۶.</summary>
public class Scheme
{
    public int Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private Scheme() { }

    public Scheme(string code, string title)
    {
        Code = Normalize(code, 2, 30, "کد طرح باید ۲ تا ۳۰ حرف باشد.");
        Title = Normalize(title, 3, 100, "نام طرح باید ۳ تا ۱۰۰ حرف باشد.");
    }

    public void Rename(string title) => Title = Normalize(title, 3, 100, "نام طرح باید ۳ تا ۱۰۰ حرف باشد.");

    public void SetActive(bool isActive) => IsActive = isActive;

    /// <summary>
    /// حذف نرم. طرح پاک نمی‌شود چون شرکت‌ها، وام‌ها و ردیف‌های گزارش تغییرات به آن اشاره دارند
    /// و با پاک شدنش مبنای محاسبهٔ اعتبارشان را از دست می‌دهند.
    /// «رد حذف آخرین طرح فعال» قاعدهٔ کاربرد است، نه موجودیت — در Handler بررسی می‌شود،
    /// چون شمردن طرح‌های فعال به دیتابیس نیاز دارد.
    /// </summary>
    public void Deactivate() => IsActive = false;

    private static string Normalize(string? value, int min, int max, string message)
    {
        value = (value ?? "").Trim();
        return value.Length >= min && value.Length <= max ? value : throw new FacilityDomainException(message);
    }
}
