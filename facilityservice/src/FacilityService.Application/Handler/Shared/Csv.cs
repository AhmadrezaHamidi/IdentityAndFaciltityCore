using System.Text;

namespace FacilityService.Application.Handler.Shared;

/// <summary>خروجی CSV با BOM تا Excel فارسی را درست باز کند.</summary>
public static class Csv
{
    public static FileUpload File(string fileName, IReadOnlyList<string> header, IEnumerable<IEnumerable<object?>> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", header.Select(Escape)));
        foreach (var row in rows) sb.AppendLine(string.Join(",", row.Select(v => Escape(Format(v)))));
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return new FileUpload(bytes, fileName, "text/csv");
    }

    private static string Format(object? v) => v switch
    {
        null => "",
        DateOnly d => Jalali.Format(d),
        DateTime t => $"{Jalali.Format(DateOnly.FromDateTime(t))} {t:HH:mm}",
        bool b => b ? "بله" : "خیر",
        decimal m => m.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture),
        IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
        _ => v.ToString() ?? "",
    };

    private static string Escape(string s)
    {
        // جلوگیری از اجرای فرمول در Excel (CSV injection)
        if (s.Length > 0 && "=+-@".Contains(s[0]) && !long.TryParse(s, out _)) s = "'" + s;
        return s.IndexOfAny([',', '"', '\n', '\r']) >= 0 ? $"\"{s.Replace("\"", "\"\"")}\"" : s;
    }
}

public static class StatusTitles
{
    public static string Of(CompanyStatus s) => s switch
    {
        CompanyStatus.Active => "فعال",
        CompanyStatus.Cancelled => "لغو قرارداد",
        CompanyStatus.Expired => "منقضی",
        _ => "حذف‌شده",
    };

    public static string Of(PersonnelStatus s) => s switch
    {
        PersonnelStatus.PendingManager => "در انتظار تأیید مدیر شرکت",
        PersonnelStatus.Approved => "تأییدشده",
        PersonnelStatus.Rejected => "ردشده",
        _ => "حذف‌شده",
    };

    public static string Of(LoanRequestStatus s) => s switch
    {
        LoanRequestStatus.PendingManager => "در انتظار تأیید مدیر شرکت",
        LoanRequestStatus.RejectedByManager => "رد توسط مدیر شرکت",
        LoanRequestStatus.PendingCredit => "در انتظار تأیید اعتبارات",
        LoanRequestStatus.RejectedByCredit => "رد توسط اعتبارات",
        LoanRequestStatus.PendingDigital => "در انتظار تأیید شعبهٔ دیجیتال",
        LoanRequestStatus.RejectedByDigital => "رد توسط شعبهٔ دیجیتال",
        LoanRequestStatus.Approved => "تأییدشده",
        _ => "لغوشده",
    };
}
