using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Employees.ReadModels;

public sealed record PersonnelDetailsDto(
    long Id, long CompanyId, int Priority, string FullName, string NationalCode, string Mobile, string PersonnelCode,
    DateOnly HireDate, PersonnelStatus Status, bool IsLocked, IReadOnlyList<PersonnelHistoryDto> History, string RowVersion);
