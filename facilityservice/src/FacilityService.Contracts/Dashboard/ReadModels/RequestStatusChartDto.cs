using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.Dashboard.ReadModels;

public sealed record RequestStatusChartDto(int Approved, int Pending, int Rejected);
