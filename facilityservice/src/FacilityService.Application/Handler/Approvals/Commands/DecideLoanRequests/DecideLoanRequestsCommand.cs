using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Approvals.Commands.DecideLoanRequests;

/// <summary>تأیید/رد تکی یا گروهی در یک مرحله.</summary>
public sealed record DecideLoanRequestsCommand : FacilityBaseCommand<FacilityBaseMessageResponse<DecisionResultDto>>
{
    [JsonIgnore, BindNever] public ApprovalStage Stage { get; init; }

    /// <summary>فیلد ۱۷۴: انتخاب تکی یا همگانی</summary>
    // = new List — نه [] : آرایهٔ ثابت‌اندازه جلوی Model Binding فرم (multipart) را می‌گیرد و مقدار خالی می‌ماند.
    [Required, MinLength(1), MaxLength(500)] public IReadOnlyList<long> RequestIds { get; init; } = new List<long>();

    [Required] public DecisionType Decision { get; init; }

    /// <summary>فیلد ۱۷۸: در رد الزامی؛ از «دلایل رد درخواست وام»</summary>
    public int? ReasonId { get; init; }

    /// <summary>فیلد ۲۵۱: تا ۱۰۰۰ حرف؛ در رد الزامی</summary>
    [StringLength(1000)] public string? Description { get; init; }

    /// <summary>پیوست سند/مصوبهٔ تصمیم (pdf, jpg, jpeg, png تا ۵ مگابایت)؛ اختیاری</summary>
    [JsonIgnore, BindNever] public FileUpload? Attachment { get; init; }

    /// <summary>
    /// «مرحلهٔ پس از تأیید»، در همین تصمیم — تکی یا گروهی.
    ///
    /// در فرانت این انتخاب داخل همان مودال تأیید است؛ قبلاً فقط سرویس جدا و فقط برای یک
    /// درخواست وجود داشت، پس تأیید گروهی با مرحله ممکن نبود و درخواست تأییدشدهٔ بدون مرحله
    /// هم می‌توانست بماند. خالی گذاشتن همان رفتار قبلی است.
    ///
    /// فقط با Decision = Approve معنی دارد، فقط برای درخواستی که با همین تصمیم به «تأییدشده»
    /// می‌رسد (اگر تأیید شعبهٔ دیجیتال فعال باشد، مرحله در همان تصمیم آخر ثبت می‌شود)،
    /// و کلید facility.credit-approval.stage لازم دارد.
    /// </summary>
    public PostApprovalStage? PostStage { get; init; }

    /// <summary>برای PostStage = RemoteGrantDenied الزامی؛ از «دلایل عدم اعطای تسهیلات غیرحضوری».</summary>
    public int? PostStageReasonId { get; init; }
}
