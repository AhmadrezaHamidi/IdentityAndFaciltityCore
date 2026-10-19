using System.ComponentModel.DataAnnotations;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Internal.Queries.InternalCompanyLookup;

/// <summary>
/// سرویس‌به‌سرویس (Identity ← Facility): Lookup شرکت‌ها برای انتساب نقش مدیر شرکت و منابع انسانی.
/// Identity این را از FacilityCompanyDirectory با هدر X-Api-Key صدا می‌زند. بدون دامنهٔ کاربر (همهٔ شرکت‌ها).
/// </summary>
public sealed record InternalCompanyLookupQuery(string? Term)
    : FacilityBaseQuery<FacilityBaseMessageResponse<IReadOnlyList<FacilityLookupItemDto>>>;
