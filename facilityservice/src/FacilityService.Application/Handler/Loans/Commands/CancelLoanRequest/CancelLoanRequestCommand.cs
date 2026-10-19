using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Loans.Commands.CancelLoanRequest;

/// <summary>US-D5: لغو درخواست یک نفر تا قبل از تأیید اعتبارات؛ دلیل از فهرست (قاعدهٔ ۸) + توضیح.</summary>
public sealed record CancelLoanRequestCommand : FacilityBaseCommand<FacilityBaseMessageResponse<LoanRequestListItemDto>>
{
    [JsonIgnore, BindNever] public long Id { get; init; }
    [Required, Range(1, int.MaxValue)] public int ReasonId { get; init; }
    [Required, StringLength(1000, MinimumLength = 5)] public string Description { get; init; } = null!;
}
