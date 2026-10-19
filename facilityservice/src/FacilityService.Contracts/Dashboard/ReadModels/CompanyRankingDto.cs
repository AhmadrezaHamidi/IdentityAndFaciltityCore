using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.Dashboard.ReadModels;

public sealed record CompanyRankingDto(int Rank, long CompanyId, string CompanyName, long? AverageBalance,
    long AllocatedAmount, int RequestCount, int PersonnelCount, string Status);
