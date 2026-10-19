using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Application.Handler.Shared;

/// <summary>پایهٔ Queryهای صفحه‌بندی‌شده.</summary>
public abstract record FacilityPagedQuery<TResponse> : FacilityBaseQuery<TResponse>
{
    [StringLength(50)] public string? Term { get; init; }
    [Range(1, int.MaxValue)] public int Page { get; init; } = 1;
    [Range(1, 100)] public int PageSize { get; init; } = 20;
}
