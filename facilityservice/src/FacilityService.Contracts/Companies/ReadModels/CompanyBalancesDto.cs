using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Companies.ReadModels;

/// <param name="Months">ماه‌های سال جاری قرارداد</param>
public sealed record CompanyBalancesDto(long CompanyId, IReadOnlyList<MonthlyBalanceDto> Months, string RowVersion);
