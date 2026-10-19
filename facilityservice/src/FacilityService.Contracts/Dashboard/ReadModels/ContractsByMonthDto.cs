using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.Dashboard.ReadModels;

/// <param name="Month">ماه شمسی «۱۴۰۵/۰۷»</param>
public sealed record ContractsByMonthDto(string Month, int CompanyCount, IReadOnlyList<string> CompanyNames);
