using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Employees.Queries.GetPersonnelImportTemplate;

/// <summary>فایل نمونهٔ اکسل. ستون‌ها: نام و نام خانوادگی، کد ملی، موبایل، کد پرسنلی، تاریخ استخدام، اولویت تخصیص.</summary>
public sealed record GetPersonnelImportTemplateQuery : FacilityBaseQuery<FacilityBaseMessageResponse<FileUpload>>;
