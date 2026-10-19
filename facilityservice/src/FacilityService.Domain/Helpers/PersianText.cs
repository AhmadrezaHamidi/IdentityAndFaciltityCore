using System.Globalization;

namespace FacilityService.Domain.Helpers;

/// <summary>متن هینت‌ها مثل نمونهٔ پنل: ارقام فارسی، جداکنندهٔ هزارگان، تومان به حروف.</summary>
public static class PersianText
{
    private const string Fa = "۰۱۲۳۴۵۶۷۸۹";

    public static string Digits(string s) => string.Concat(s.Select(c => c is >= '0' and <= '9' ? Fa[c - '0'] : c));

    public static string Number(long n) => Digits(n.ToString("#,0", CultureInfo.InvariantCulture));

    public static long Toman(long rial) => (long)Math.Round(rial / 10m, MidpointRounding.AwayFromZero);

    public static string Rial(long n) => $"{Number(n)} ریال ({Number(Toman(n))} تومان)";

    public static string TomanWords(long rial) => $"{Words(Toman(rial))} تومان";

    private static readonly string[] Ones = ["", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه"];
    private static readonly string[] Teens = ["ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده"];
    private static readonly string[] Tens = ["", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود"];
    private static readonly string[] Hundreds = ["", "صد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد"];
    private static readonly string[] Scales = ["", "هزار", "میلیون", "میلیارد", "تریلیون"];

    public static string Words(long n)
    {
        if (n == 0) return "صفر";
        if (n < 0) return "منفی " + Words(-n);
        var groups = new List<int>();
        for (var x = n; x > 0; x /= 1000) groups.Add((int)(x % 1000));
        var parts = new List<string>();
        for (var i = groups.Count - 1; i >= 0; i--)
        {
            if (groups[i] == 0) continue;
            parts.Add(ThreeDigits(groups[i]) + (i < Scales.Length && Scales[i] != "" ? " " + Scales[i] : ""));
        }
        return string.Join(" و ", parts);
    }

    private static string ThreeDigits(int num)
    {
        var parts = new List<string>();
        if (num / 100 > 0) parts.Add(Hundreds[num / 100]);
        var rem = num % 100;
        if (rem is >= 10 and < 20) parts.Add(Teens[rem - 10]);
        else
        {
            if (rem / 10 > 0) parts.Add(Tens[rem / 10]);
            if (rem % 10 > 0) parts.Add(Ones[rem % 10]);
        }
        return string.Join(" و ", parts);
    }
}
