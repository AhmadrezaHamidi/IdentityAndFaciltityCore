using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Employees.ReadModels;

/// <summary>US-E3: «وضعیت‌های تغییر یافته را به همراه تاریخ و ساعت مشاهده کند».</summary>
public sealed record PersonnelHistoryDto(PersonnelStatus? From, PersonnelStatus To, string? Reason, string? Description, RegistrarDto By, DateTime At);
