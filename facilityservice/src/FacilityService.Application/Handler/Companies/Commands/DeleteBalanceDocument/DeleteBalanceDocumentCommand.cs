using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Commands.DeleteBalanceDocument;

/// <summary>فقط ادمین (معیار پذیرش ۳).</summary>
public sealed record DeleteBalanceDocumentCommand(long CompanyId, long DocumentId) : FacilityBaseCommand<FacilityBaseMessageResponse<bool>>;
