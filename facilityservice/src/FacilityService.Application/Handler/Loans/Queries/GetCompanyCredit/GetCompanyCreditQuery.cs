using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Loans.Queries.GetCompanyCredit;

/// <summary>اعتبار و بنر وضعیت شرکت (US-E4، D2). BaseDate پیش‌فرض امروز.</summary>
public sealed record GetCompanyCreditQuery(long CompanyId, DateOnly? BaseDate)
    : FacilityBaseQuery<FacilityBaseMessageResponse<CompanyCreditDto>>;
