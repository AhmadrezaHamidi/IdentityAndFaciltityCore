using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.Dashboard.ReadModels;

public sealed record MonthlyAllocationDto(long CompanyId, string CompanyName, IReadOnlyDictionary<string, long> AmountByMonth);
