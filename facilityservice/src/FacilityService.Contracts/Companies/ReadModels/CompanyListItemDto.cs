using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Companies.ReadModels;

public sealed record CompanyListItemDto(
    long Id, string Name, string NationalId, string AgreementNumber, DateOnly ContractDate,
    int PlanCount, RegistrarDto Registrar, CompanyStatus Status, string BranchCode,
    DateOnly? CancelDate, string? CancelReasonTitle);
