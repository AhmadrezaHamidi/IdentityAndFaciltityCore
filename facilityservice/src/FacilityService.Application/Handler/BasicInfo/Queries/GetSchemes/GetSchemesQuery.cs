using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Queries.GetSchemes;

/// <summary>طرح‌های کلان (نگین میثاق، شجره طیبه، هم‌پیمان، ...) — فیلد ۲۰۶. اول این انتخاب می‌شود.</summary>
public sealed record GetSchemesQuery : FacilityBaseQuery<FacilityBaseMessageResponse<IReadOnlyList<SchemeDto>>>;
