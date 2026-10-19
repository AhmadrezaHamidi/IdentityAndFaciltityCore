using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Companies.ReadModels;

/// <param name="ActiveCount">سرتیتر نمونه: «X شرکت فعال از Y شرکت ثبت‌شده» (در دامنهٔ کاربر)</param>
public sealed record CompanySearchResultDto(FacilityPagedResponse<CompanyListItemDto> Page, int ActiveCount, int RegisteredCount);
