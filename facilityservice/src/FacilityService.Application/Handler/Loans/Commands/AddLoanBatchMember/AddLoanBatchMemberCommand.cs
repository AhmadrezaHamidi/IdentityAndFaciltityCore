using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Loans.Commands.AddLoanBatchMember;

/// <summary>US-D4: افزودن فرد به یک ثبت (قبل از تأیید اعتبارات).</summary>
public sealed record AddLoanBatchMemberCommand : FacilityBaseCommand<FacilityBaseMessageResponse<LoanBatchDto>>
{
    [JsonIgnore, BindNever] public long BatchId { get; init; }
    [Required] public LoanRequestItemInput Item { get; init; } = null!;
}
