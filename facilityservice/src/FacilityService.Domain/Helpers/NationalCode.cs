namespace FacilityService.Domain.Helpers;

/// <summary>کد ملی ایران: ۱۰ رقم با رقم کنترل (F-0.4.2).</summary>
public static class NationalCode
{
    public static bool IsValid(string? code)
    {
        if (code is not { Length: 10 } || !code.All(char.IsAsciiDigit)) return false;
        if (code.Distinct().Count() == 1) return false; // ۰۰۰۰۰۰۰۰۰۰، ۱۱۱۱۱۱۱۱۱۱ و ...
        return code[9] - '0' == CheckDigit(code[..9]);
    }

    /// <summary>رقم کنترل برای ۹ رقم اول (برای ساخت دادهٔ نمونه).</summary>
    public static int CheckDigit(string firstNine)
    {
        var sum = 0;
        for (var i = 0; i < 9; i++) sum += (firstNine[i] - '0') * (10 - i);
        var r = sum % 11;
        return r < 2 ? r : 11 - r;
    }
}
