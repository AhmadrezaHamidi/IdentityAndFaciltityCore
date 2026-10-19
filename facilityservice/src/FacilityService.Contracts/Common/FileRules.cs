using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Common;

public static class FileRules
{
    public const long FiveMb = 5 * 1024 * 1024;
    public const long TenMb = 10 * 1024 * 1024;
    public const long OneKb = 1024;

    public static readonly string[] AgreementExtensions = [".pdf", ".jpg", ".jpeg", ".png", ".zip"];          // فیلد ۲۲۳
    public static readonly string[] BalanceDocumentExtensions = [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".zip", ".jpg", ".png"];
    public static readonly string[] LetterExtensions = [".pdf", ".jpg", ".jpeg", ".png"];                      // لغو، حذف پرسنل، تصمیم
    public static readonly string[] PersonnelImportExtensions = [".xlsx", ".xls", ".csv"];                     // فیلد ۲۴۰

    /// <summary>null یعنی معتبر.</summary>
    /// <param name="minSize">کمترین حجم؛ پیش‌فرض ۱ کیلوبایت برای مستندات. فایل CSV چندردیفی کمتر از ۱ کیلوبایت است ← برای بارگذاری پرسنل ۱ بایت.</param>
    public static string? Validate(FileUpload? file, string[] extensions, long max, bool required, long minSize = OneKb)
    {
        if (file is null) return required ? "الصاق فایل الزامی است." : null;
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!extensions.Contains(ext)) return $"فرمت فایل مجاز نیست. مجاز: {string.Join("، ", extensions)}";
        if (file.Length < Math.Max(1, minSize)) return "فایل خالی یا خیلی کوچک است.";
        if (file.Length > max) return $"حجم فایل نباید بیشتر از {max / (1024 * 1024)} مگابایت باشد.";
        return null;
    }
}
