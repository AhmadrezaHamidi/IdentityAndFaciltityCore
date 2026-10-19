using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Approvals.Queries.GetPendingCounts;

/// <summary>شمارهٔ روی تب‌ها (قاعدهٔ ۱: «تعداد درخواست‌های در انتظار روی تب»). فقط مراحلی که کاربر مجوز دیدنشان را دارد.</summary>
public sealed record GetPendingCountsQuery : FacilityBaseQuery<FacilityBaseMessageResponse<PendingCountsDto>>;
