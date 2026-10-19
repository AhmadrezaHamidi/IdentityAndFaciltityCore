using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Employees.Commands.ImportPersonnel;

/// <summary>US-E2 بارگذاری گروهی (فیلد ۲۴۰: xlsx, xls, csv تا ۱۰ مگابایت). جایگزین BulkUserImport/UploadExel.</summary>
public sealed record ImportPersonnelCommand : FacilityBaseCommand<FacilityBaseMessageResponse<PersonnelImportResultDto>>
{
    [JsonIgnore, BindNever] public long CompanyId { get; init; }
    [JsonIgnore, BindNever] public FileUpload? File { get; init; }
}
