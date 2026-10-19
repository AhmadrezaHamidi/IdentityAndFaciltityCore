using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Commands.DeleteReason;

/// <summary>حذف نرم (غیرفعال)؛ رکوردهای قدیمی که این دلیل را دارند سالم می‌مانند.</summary>
public sealed record DeleteReasonCommand(int SchemeId, int Id) : FacilityBaseCommand<FacilityBaseMessageResponse<bool>>;
