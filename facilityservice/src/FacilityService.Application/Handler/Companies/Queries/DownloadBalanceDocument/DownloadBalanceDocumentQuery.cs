using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Queries.DownloadBalanceDocument;

public sealed record DownloadBalanceDocumentQuery(long CompanyId, long DocumentId) : FacilityBaseQuery<FacilityBaseMessageResponse<FileUpload>>;
