using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Shared;

/// <summary>آنچه برای بازگردانی میانگین‌ها در گزارش تغییرات نگه داشته می‌شود.</summary>
public sealed record BalancesRestorePayload(long CompanyId, IReadOnlyDictionary<string, long> Balances);
