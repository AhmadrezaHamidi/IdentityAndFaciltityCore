using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Commands.DeleteCompany;

/// <summary>حذف شرکت (نرم؛ در داشبورد «شرکت‌های حذف‌شده» می‌ماند). فقط وقتی درخواست تأییدشده ندارد.</summary>
public sealed record DeleteCompanyCommand : FacilityBaseCommand<FacilityBaseMessageResponse<bool>>
{
    [JsonIgnore, BindNever] public long Id { get; init; }
    [Required, StringLength(500, MinimumLength = 5)] public string Description { get; init; } = null!;
}
