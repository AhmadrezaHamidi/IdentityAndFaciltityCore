using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Employees.Commands.DeletePersonnel;

/// <summary>حذف: توضیح (فیلد ۱۳۱، ۵ تا ۵۰۰) و فایل (فیلد ۱۳۳، pdf/jpg/png تا ۵ مگابایت) الزامی.</summary>
public sealed record DeletePersonnelCommand : FacilityBaseCommand<FacilityBaseMessageResponse<bool>>
{
    [JsonIgnore, BindNever] public long Id { get; init; }
    [Required, StringLength(500, MinimumLength = 5)] public string Description { get; init; } = null!;
    [JsonIgnore, BindNever] public FileUpload? File { get; init; }
}
