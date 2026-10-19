using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Employees.ReadModels;

public sealed record PersonnelImportResultDto(long BatchId, int TotalRows, int Accepted, int Rejected, IReadOnlyList<PersonnelImportErrorDto> Errors);
