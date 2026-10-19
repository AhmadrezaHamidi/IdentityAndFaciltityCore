using System.ComponentModel.DataAnnotations;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Dashboard.Queries.GetDashboard;

public sealed record GetDashboardQuery : FacilityBaseQuery<FacilityBaseMessageResponse<DashboardDto>>
{
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public long? CompanyId { get; init; }
    [StringLength(50)] public string? BranchCode { get; init; }
}
