using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Queries.SearchCompanies;

public sealed record SearchCompaniesQuery : FacilityPagedQuery<FacilityBaseMessageResponse<CompanySearchResultDto>>
{
    public CompanyStatus? Status { get; init; }

    /// <summary>فقط برای ادمین/دیجیتال معنی دارد؛ برای کاربر شعبه نادیده گرفته می‌شود (دامنه‌اش ثابت است).</summary>
    [StringLength(50)] public string? BranchCode { get; init; }
}
