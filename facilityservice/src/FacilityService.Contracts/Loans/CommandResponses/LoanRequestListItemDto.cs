using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Loans.CommandResponses;

public sealed record LoanRequestListItemDto(
    long Id, long BatchId, long CompanyId, string CompanyName, string BranchCode, long PersonnelId, string PersonnelName, string PersonnelCode,
    string PlanTitle, long Amount, int PaymentOrder, DateTime CreatedAt, DateTime? DecidedAt,
    LoanRequestStatus Status, PostApprovalStage? PostStage, string? Description, int DocumentCount, bool CanCancel,
    IReadOnlyList<StoredFileDto> Documents);
