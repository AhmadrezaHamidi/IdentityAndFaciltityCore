using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Approvals.Commands.SetPostApprovalStage;

/// <summary>تغییر «مرحلهٔ پس از تأیید» برای درخواست تأییدشده (نمونهٔ پنل). برای «عدم اعطای غیرحضوری» دلیل الزامی است.</summary>
public sealed record SetPostApprovalStageCommand : FacilityBaseCommand<FacilityBaseMessageResponse<LoanRequestListItemDto>>
{
    [JsonIgnore, BindNever] public long RequestId { get; init; }
    [Required] public PostApprovalStage Stage { get; init; }

    /// <summary>از «دلایل عدم اعطای تسهیلات غیرحضوری»</summary>
    public int? ReasonId { get; init; }

    [StringLength(1000)] public string? Description { get; init; }
}
