using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Loans.ReadModels;

/// <param name="PersonnelId">فردی که در همین ماه قبلاً وام گرفته.</param>
/// <param name="Month">ماه شمسی تخصیص قبلی، قالب 1404/05.</param>
/// <param name="PreviousAmount">مجموع مبالغ تخصیص‌های همان ماه این فرد.</param>
public sealed record RepeatAllocationDto(long PersonnelId, string PersonnelName, string PersonnelCode, string Month, long PreviousAmount);

/// <param name="Messages">پیام‌های هینت به ترتیب نمایش (سبز/قرمز/راهنما)</param>
/// <param name="RepeatAllocations">
/// افرادی از فهرست انتخاب‌شده که در همان ماه تاریخ مبنا قبلاً وام گرفته‌اند (قاعدهٔ «هشدار
/// تخصیص مجدد در همان ماه»). هشدار است نه خطا: ثبت را قفل نمی‌کند و <c>IsAllowed</c> را
/// عوض نمی‌کند — فرانت فهرست را نشان می‌دهد و تأیید دوباره می‌گیرد.
/// درخواست لغوشده یا ردشده حساب نمی‌شود، چون اعتباری مصرف نکرده.
/// </param>
public sealed record LoanRequestPreviewDto(
    int SelectedCount, long Total, long RemainingCredit, bool IsAllowed, int? MaxCoverablePeople,
    long? SuggestedMaxAmount, IReadOnlyList<HintDto> Messages,
    IReadOnlyList<RepeatAllocationDto> RepeatAllocations);
