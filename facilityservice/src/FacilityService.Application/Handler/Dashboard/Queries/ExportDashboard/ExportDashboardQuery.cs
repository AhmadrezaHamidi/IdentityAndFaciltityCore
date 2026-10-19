using System.ComponentModel.DataAnnotations;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Dashboard.Queries.ExportDashboard;

public sealed record ExportDashboardQuery(DashboardExport Report) : FacilityBaseQuery<FacilityBaseMessageResponse<FileUpload>>;
