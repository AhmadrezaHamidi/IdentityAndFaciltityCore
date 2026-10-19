using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Companies.ReadModels;

public sealed record CompanyDetailsDto(
    long Id, int SchemeId, string Name, string NationalId, string AgreementNumber, string AccountNumber,
    DateOnly ContractDate, DateOnly AccountOpeningDate, int AccountOpeningDayOfMonth, bool SalaryFromSepah,
    IReadOnlyList<PlanRefDto> Plans, StoredFileDto? AgreementFile,
    RegistrarDto Registrar, DateTime CreatedAt, CompanyStatus Status, CancellationInfoDto? Cancellation,
    string BranchCode, string RowVersion);
