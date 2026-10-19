using System.ComponentModel.DataAnnotations;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.ChangeLogs.Queries.SearchChangeLogs;

public sealed record SearchChangeLogsQuery : FacilityPagedQuery<FacilityBaseMessageResponse<FacilityPagedResponse<ChangeLogItemDto>>>
{
    public ChangeLogCategory? Category { get; init; }
    public long? CompanyId { get; init; }
    public int? PanelUserId { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
}
