using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.Dashboard.ReadModels;

public sealed record PlanShareDto(int PlanId, string PlanTitle, int RequestCount);
