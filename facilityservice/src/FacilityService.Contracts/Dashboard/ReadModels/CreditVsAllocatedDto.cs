using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.Dashboard.ReadModels;

public sealed record CreditVsAllocatedDto(string GroupId, string GroupTitle, long CreatedCredit, long Allocated);

/// <summary>
/// «گروه» در نمودار اعتبار در برابر تخصیص چیست. فرانت نمی‌دانست شعبه است، طرح است یا شرکت —
/// و جواب به دامنهٔ کاربر بستگی دارد، پس باید در خود پاسخ بیاید.
/// </summary>
public enum DashboardGroupKind : byte
{
    /// <summary>به تفکیک شعبه (کاربری که داشبورد را سراسری می‌بیند: ادمین و شعبهٔ دیجیتال).</summary>
    Branch = 1,

    /// <summary>به تفکیک شرکت (کاربر شعبه و نقش‌های شرکت).</summary>
    Company = 2,
}

/// <param name="Key">کلید ماه شمسی، همیشه به قالب <c>1404/05</c> (چهار رقم سال، اسلش، دو رقم ماه).</param>
/// <param name="Title">عنوان فارسی برای نمایش، مثل «مرداد ۱۴۰۴».</param>
public sealed record MonthOptionDto(string Key, string Title);
