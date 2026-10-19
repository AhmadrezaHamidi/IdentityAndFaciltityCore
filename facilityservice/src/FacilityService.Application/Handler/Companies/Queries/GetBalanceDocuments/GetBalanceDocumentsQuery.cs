using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Queries.GetBalanceDocuments;

public sealed record GetBalanceDocumentsQuery(long CompanyId, string? Month)
    : FacilityBaseQuery<FacilityBaseMessageResponse<IReadOnlyList<BalanceDocumentDto>>>;
