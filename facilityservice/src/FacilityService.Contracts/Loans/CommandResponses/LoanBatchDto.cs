using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Loans.CommandResponses;

public sealed record LoanBatchDto(long BatchId, long CompanyId, int PlanId, AllocationMode Mode, long Total, IReadOnlyList<LoanRequestListItemDto> Requests);
