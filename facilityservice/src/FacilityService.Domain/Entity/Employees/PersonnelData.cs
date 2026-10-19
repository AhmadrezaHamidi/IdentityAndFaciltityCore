using System.Text.RegularExpressions;

namespace FacilityService.Domain.Entity.Employees;

/// <summary>مشخصات پرسنل (فیلدهای ۲۴۱ تا ۲۴۶) — هم فرم و هم هر ردیف فایل اکسل.</summary>
public sealed record PersonnelData(string FullName, string NationalCode, string Mobile, string PersonnelCode, DateOnly HireDate, int Priority)
{
    public static readonly DateOnly MinHireDate = Jalali.ToDate(1350, 1, 1);

    public PersonnelData Normalize(DateOnly today)
    {
        var d = this with
        {
            FullName = (FullName ?? "").Trim(),
            NationalCode = (NationalCode ?? "").Trim(),
            Mobile = (Mobile ?? "").Trim(),
            PersonnelCode = (PersonnelCode ?? "").Trim(),
        };
        if (!Regex.IsMatch(d.FullName, @"^[؀-ۿ‌\s]{3,60}$")) throw new FacilityDomainException("نام و نام خانوادگی: ۳ تا ۶۰ حرف فارسی.");
        if (!Regex.IsMatch(d.NationalCode, @"^\d{10}$")) throw new FacilityDomainException("کد ملی باید ۱۰ رقم باشد.");
        if (!Helpers.NationalCode.IsValid(d.NationalCode)) throw new FacilityDomainException("کد ملی معتبر نیست.");
        if (!Regex.IsMatch(d.Mobile, @"^09\d{9}$")) throw new FacilityDomainException("شماره موبایل باید ۱۱ رقم و با ۰۹ شروع شود.");
        if (!Regex.IsMatch(d.PersonnelCode, @"^[a-zA-Z0-9\-_]{1,20}$")) throw new FacilityDomainException("کد پرسنلی: ۱ تا ۲۰ حرف لاتین، عدد، - و _.");
        if (d.HireDate > today) throw new FacilityDomainException("تاریخ استخدام نمی‌تواند در آینده باشد.");
        if (d.HireDate < MinHireDate) throw new FacilityDomainException("تاریخ استخدام باید بعد از ۱۳۵۰/۰۱/۰۱ باشد.");
        if (d.Priority is < 1 or > 999) throw new FacilityDomainException("اولویت تخصیص باید بین ۱ تا ۹۹۹ باشد.");
        return d;
    }
}
