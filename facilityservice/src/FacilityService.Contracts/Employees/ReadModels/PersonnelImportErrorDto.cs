using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Employees.ReadModels;

public sealed record PersonnelImportErrorDto(int Row, string Column, string Message);
