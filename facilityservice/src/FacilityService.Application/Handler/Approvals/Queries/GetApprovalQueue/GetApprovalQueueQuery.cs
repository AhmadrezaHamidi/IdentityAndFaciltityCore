using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Approvals.Queries.GetApprovalQueue;

/// <summary>صف هر مرحله. CompanyId اختیاری (فیلتر «همهٔ شرکت‌ها» فیلد ۲۵۰)؛ دامنهٔ کاربر همیشه اعمال می‌شود.</summary>
public sealed record GetApprovalQueueQuery : FacilityPagedQuery<FacilityBaseMessageResponse<FacilityPagedResponse<LoanRequestListItemDto>>>
{
    [JsonIgnore, BindNever] public ApprovalStage Stage { get; init; }
    public long? CompanyId { get; init; }

    /// <summary>null = فقط منتظر تصمیم؛ برای دیدن سابقه (تأییدشده/ردشده) وضعیت را بده.</summary>
    public LoanRequestStatus? Status { get; init; }
}
