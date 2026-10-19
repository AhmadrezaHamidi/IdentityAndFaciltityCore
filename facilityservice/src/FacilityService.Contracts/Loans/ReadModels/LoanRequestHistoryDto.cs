using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Loans.ReadModels;

/// <summary>قاعدهٔ فرایندی ۵: تاریخ تصمیم، تصمیم‌گیرنده، وضعیت قبلی و جدید.</summary>
public sealed record LoanRequestHistoryDto(LoanRequestStatus? From, LoanRequestStatus To, PostApprovalStage? Stage,
    string? Reason, string? Description, RegistrarDto By, DateTime At);
