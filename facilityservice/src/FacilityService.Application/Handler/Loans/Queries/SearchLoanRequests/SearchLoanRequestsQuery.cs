using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Loans.Queries.SearchLoanRequests;

public sealed record SearchLoanRequestsQuery : FacilityPagedQuery<FacilityBaseMessageResponse<FacilityPagedResponse<LoanRequestListItemDto>>>
{
    /// <summary>«درخواست‌های وام این شرکت» در تب تخصیص وام</summary>
    public long? CompanyId { get; init; }
    [StringLength(50)] public string? BranchCode { get; init; }
    public LoanRequestStatus? Status { get; init; }
    public int? PlanId { get; init; }
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
}
