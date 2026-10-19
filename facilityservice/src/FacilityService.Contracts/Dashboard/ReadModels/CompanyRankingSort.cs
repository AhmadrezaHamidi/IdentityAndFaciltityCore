using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.Dashboard.ReadModels;

public enum CompanyRankingSort : byte { AverageBalance = 1, AllocatedAmount = 2, RequestCount = 3, PersonnelCount = 4 }
