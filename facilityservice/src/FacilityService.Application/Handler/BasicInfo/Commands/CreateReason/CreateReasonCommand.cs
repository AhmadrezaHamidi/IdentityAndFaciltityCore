using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Commands.CreateReason;

public sealed record CreateReasonCommand : FacilityBaseCommand<FacilityBaseMessageResponse<ReasonDto>>
{
    [Required] public ReasonType Type { get; init; }

    /// <summary>فیلد ۴۶: ۳ تا ۱۰۰ حرف فارسی</summary>
    [Required, StringLength(100, MinimumLength = 3)] public string Title { get; init; } = null!;
}
