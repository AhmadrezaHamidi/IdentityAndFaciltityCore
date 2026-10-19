using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Commands.UpdateReason;

public sealed record UpdateReasonCommand : FacilityBaseCommand<FacilityBaseMessageResponse<ReasonDto>>
{
    [JsonIgnore, BindNever] public int Id { get; init; }
    [Required, StringLength(100, MinimumLength = 3)] public string Title { get; init; } = null!;
    public bool IsActive { get; init; } = true;
    [Range(0, 1000)] public int SortOrder { get; init; }
}
