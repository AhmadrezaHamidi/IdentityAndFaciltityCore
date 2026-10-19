using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Commands.UpdateCompanyBalances;

/// <summary>ذخیرهٔ میانگین‌ها (فرانت قبلش تأیید می‌گیرد). مبلغ قبلی و جدید در گزارش تغییرات ثبت می‌شود.</summary>
public sealed record UpdateCompanyBalancesCommand : FacilityBaseCommand<FacilityBaseMessageResponse<CompanyBalancesDto>>
{
    [JsonIgnore, BindNever] public long CompanyId { get; init; }
    [Required, MinLength(1), MaxLength(24)] public IReadOnlyList<MonthlyBalanceInput> Months { get; init; } = [];
    [Required] public string RowVersion { get; init; } = null!;
}
