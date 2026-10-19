using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Employees.Shared;

public sealed record PersonnelRestorePayload(long PersonnelId, PersonnelStatus Status);
