using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Loans.ReadModels;

/// <param name="Status">وضعیت واجد شرایط بودن؛ عنوان فارسی‌اش در EnumCatalog/EligibilityStatus است.</param>
/// <param name="Message">
/// پیام اصلی بنر. برای سازگاری نگه داشته شده؛ همان عضو اول <paramref name="Messages"/> است.
/// </param>
/// <param name="AllowedDayOfMonth">روز مجاز انتقال اعتبار در ماه (مطابق روز افتتاح حساب سپرده)؛ اگر قرارداد فعال نباشد null.</param>
/// <param name="CanRegisterLoan">
/// فرانت دکمهٔ «ثبت درخواست وام» را با همین فعال/غیرفعال می‌کند و قاعدهٔ
/// <c>status == 3</c> را در کد خودش تکرار نمی‌کند — اگر فردا وضعیت پنجمی اضافه شد
/// که اجازه می‌دهد، فرانت نمی‌شکند.
/// </param>
/// <param name="Messages">
/// همهٔ پیام‌های بنر. امروز همیشه یک عضو دارد؛ آرایه است تا افزودن پیام دوم
/// (مثلاً هشدار نزدیک بودن سقف) قرارداد را عوض نکند.
/// </param>
public sealed record EligibilityBannerDto(
    EligibilityStatus Status,
    string Message,
    int? AllowedDayOfMonth,
    bool CanRegisterLoan,
    IReadOnlyList<string> Messages);
