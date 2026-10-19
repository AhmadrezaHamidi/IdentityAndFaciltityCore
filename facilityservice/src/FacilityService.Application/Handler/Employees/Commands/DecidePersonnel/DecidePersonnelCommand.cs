using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Employees.Commands.DecidePersonnel;

/// <summary>US-D1: تأیید/رد تکی یا گروهی افراد معرفی‌شده توسط مدیر شرکت؛ همه در یک تراکنش.</summary>
public sealed record DecidePersonnelCommand : FacilityBaseCommand<FacilityBaseMessageResponse<int>>
{
    [Required, MinLength(1), MaxLength(500)] public IReadOnlyList<long> PersonnelIds { get; init; } = [];
    [Required] public DecisionType Decision { get; init; }

    /// <summary>از «دلایل رد پرسنل»؛ برای رد الزامی ⚠ سند برای رد پرسنل «توضیح اختیاری» گفته</summary>
    public int? ReasonId { get; init; }

    [StringLength(1000)] public string? Description { get; init; }
}
