using System.ComponentModel.DataAnnotations;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Dashboard.Queries.GetCompanyRanking;

/// <summary>رتبه‌بندی شرکت‌ها. Month خالی = همهٔ ماه‌ها (تجمعی)؛ با ماه، ستون وام و تعداد درخواست فقط همان ماه.</summary>
public sealed record GetCompanyRankingQuery : FacilityBaseQuery<FacilityBaseMessageResponse<IReadOnlyList<CompanyRankingDto>>>
{
    [RegularExpression(@"^\d{4}/\d{2}$")] public string? Month { get; init; }
    public CompanyRankingSort SortBy { get; init; } = CompanyRankingSort.AverageBalance;
    [Range(1, 500)] public int Top { get; init; } = 50;
}
