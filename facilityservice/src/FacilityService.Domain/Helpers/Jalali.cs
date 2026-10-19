using System.Globalization;

namespace FacilityService.Domain.Helpers;

/// <summary>تاریخ شمسی برای قواعد طرح (ماه قرارداد، روز انتقال اعتبار، ماه‌های میانگین). ذخیره همیشه میلادی است.</summary>
public static class Jalali
{
    private static readonly PersianCalendar Calendar = new();

    public static (int Year, int Month, int Day) Of(DateOnly date)
    {
        var dt = date.ToDateTime(TimeOnly.MinValue);
        return (Calendar.GetYear(dt), Calendar.GetMonth(dt), Calendar.GetDayOfMonth(dt));
    }

    public static DateOnly ToDate(int year, int month, int day) =>
        DateOnly.FromDateTime(Calendar.ToDateTime(year, month, Math.Min(day, Calendar.GetDaysInMonth(year, month)), 0, 0, 0, 0));

    public static int DayOf(DateOnly date) => Of(date).Day;

    /// <summary>«۱۴۰۵/۰۷» با ارقام لاتین (کلید ماه در API و دیتابیس).</summary>
    public static string MonthKey(DateOnly date)
    {
        var (y, m, _) = Of(date);
        return MonthKey(y, m);
    }

    public static string MonthKey(int year, int month) => $"{year:D4}/{month:D2}";

    public static bool TryParseMonthKey(string? key, out int year, out int month)
    {
        year = month = 0;
        if (key is not { Length: 7 } || key[4] != '/') return false;
        return int.TryParse(key[..4], out year) && int.TryParse(key[5..], out month) && month is >= 1 and <= 12;
    }

    /// <summary>n ماه شمسی جلو/عقب با حفظ روز (اگر ماه مقصد کوتاه‌تر است، آخرین روزش).</summary>
    public static DateOnly AddMonths(DateOnly date, int months)
    {
        var (y, m, d) = Of(date);
        var index = y * 12 + (m - 1) + months;
        return ToDate(index / 12, index % 12 + 1, d);
    }

    /// <summary>تعداد ماه‌های کامل شمسی از from تا to.</summary>
    public static int WholeMonthsBetween(DateOnly from, DateOnly to)
    {
        var (fy, fm, fd) = Of(from);
        var (ty, tm, td) = Of(to);
        var months = (ty - fy) * 12 + (tm - fm);
        return td < fd ? months - 1 : months;
    }

    /// <summary>
    /// شروع سال جاری قرارداد در تاریخ on. قرارداد تا لغو نشده هر سال خودکار تمدید می‌شود
    /// (نمونهٔ پنل: «فعال (تمدید خودکار فعال)»)؛ پیش از تاریخ انعقاد همان سال اول است.
    /// </summary>
    public static DateOnly ContractYearStart(DateOnly contractDate, DateOnly on) =>
        AddMonths(contractDate, 12 * (Math.Max(0, WholeMonthsBetween(contractDate, on)) / 12));

    /// <summary>۱۲ ماه شمسی سال جاری قرارداد در تاریخ on (ردیف‌های جدول میانگین موجودی).</summary>
    public static IReadOnlyList<string> ContractMonths(DateOnly contractDate, DateOnly on) =>
        ContractMonths(ContractYearStart(contractDate, on));

    /// <summary>
    /// روز مجاز انتقال اعتبار در ماه شمسی تاریخ date: همان روز افتتاح حساب؛ اگر ماه کوتاه‌تر است (۳۰ و ۳۱)، آخرین روز ماه.
    /// </summary>
    public static int TransferDayIn(DateOnly accountOpeningDate, DateOnly date)
    {
        var (y, m, _) = Of(date);
        return Math.Min(DayOf(accountOpeningDate), Calendar.GetDaysInMonth(y, m));
    }

    /// <summary>کلید ماه شمسی قبل از ماه date.</summary>
    public static string PreviousMonthKey(DateOnly date)
    {
        var (y, m, _) = Of(date);
        return m == 1 ? MonthKey(y - 1, 12) : MonthKey(y, m - 1);
    }

    /// <summary>۱۲ ماه شمسی از ماه شروع (ماه انعقاد یا شروع سال تمدیدشده).</summary>
    public static IReadOnlyList<string> ContractMonths(DateOnly yearStart)
    {
        var (y, m, _) = Of(yearStart);
        return Enumerable.Range(0, 12).Select(i =>
        {
            var index = y * 12 + (m - 1) + i;
            return MonthKey(index / 12, index % 12 + 1);
        }).ToList();
    }

    public static string Format(DateOnly date)
    {
        var (y, m, d) = Of(date);
        return PersianText.Digits($"{y:D4}/{m:D2}/{d:D2}");
    }
}
