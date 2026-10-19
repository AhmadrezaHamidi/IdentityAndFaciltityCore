using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Queries.GetCompanyLookup;

/// <summary>Dropdown «انتخاب شرکت» در همهٔ تب‌ها؛ فقط شرکت‌های دامنهٔ کاربر (فیلد ۲۲۴ و ۲۵۰).</summary>
public sealed record GetCompanyLookupQuery(string? Term, bool ActiveOnly = true)
    : FacilityBaseQuery<FacilityBaseMessageResponse<IReadOnlyList<FacilityLookupItemDto>>>;

// ── میانگین موجودی حساب جاری (US-B4) و مستندات (US-B5) ──
