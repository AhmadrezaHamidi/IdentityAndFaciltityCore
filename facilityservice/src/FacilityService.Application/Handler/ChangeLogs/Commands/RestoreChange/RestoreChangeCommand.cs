using System.ComponentModel.DataAnnotations;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.ChangeLogs.Commands.RestoreChange;

/// <summary>بازگردانی یک عملیات (US-A4). ⚠ Q16: فهرست عملیات قابل بازگردانی باید تعیین شود.</summary>
public sealed record RestoreChangeCommand : FacilityBaseCommand<FacilityBaseMessageResponse<ChangeLogItemDto>>
{
    [System.Text.Json.Serialization.JsonIgnore, BindNever] public long Id { get; init; }
    [Required, StringLength(500, MinimumLength = 5)] public string Description { get; init; } = null!;
}
