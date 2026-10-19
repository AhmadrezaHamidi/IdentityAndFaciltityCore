using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Queries.GetReasons;

/// <summary>
/// یک فهرست داینامیک از یک طرح. فهرست‌ها دیگر سراسری نیستند، پس طرح لازم است.
///
/// <paramref name="SchemeId"/> صفر یعنی «طرح پیش‌فرض»: اگر فقط یک طرح فعال باشد همان،
/// وگرنه خطا. این فقط برای سازگاری با فراخوان‌های قدیمی است و به محض افزودن طرح دوم
/// با پیام روشن می‌شکند، نه بی‌صدا.
/// </summary>
public sealed record GetReasonsQuery(int SchemeId, ReasonType Type, bool IncludeInactive = false)
    : FacilityBaseQuery<FacilityBaseMessageResponse<IReadOnlyList<ReasonDto>>>;
