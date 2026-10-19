using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Queries.GetPlans;

/// <summary>طرح‌های قابل انتخاب (ترکیب نوع عقد × نرخ × مدت) برای فرم شرکت و تخصیص وام.</summary>
public sealed record GetPlansQuery(int SchemeId) : FacilityBaseQuery<FacilityBaseMessageResponse<PlanListDto>>;
