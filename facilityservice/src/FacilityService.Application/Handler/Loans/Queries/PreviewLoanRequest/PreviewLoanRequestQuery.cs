using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Loans.Queries.PreviewLoanRequest;

/// <summary>
/// پیش‌نمایش و اعتبارسنجی زندهٔ فرم (هینت‌ها): مجموع، مجاز بودن، «حداکثر تا N نفر با این مبلغ».
/// فرانت هم محاسبه می‌کند؛ این Endpoint همان قواعد سرور را برمی‌گرداند تا ناهماهنگ نشوند.
/// </summary>
public sealed record PreviewLoanRequestQuery : FacilityBaseQuery<FacilityBaseMessageResponse<LoanRequestPreviewDto>>
{
    [JsonIgnore, BindNever] public long CompanyId { get; init; }
    [Required, Range(1, int.MaxValue)] public int PlanId { get; init; }
    public DateOnly? BaseDate { get; init; }
    [Required] public AllocationMode Mode { get; init; }
    [Range(0, 4_000_000_000)] public long? Amount { get; init; }
    public IReadOnlyList<LoanRequestItemInput> Items { get; init; } = [];
}
