using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.BasicInfo.ReadModels;

public sealed record SchemeDto(int Id, string Code, string Title, bool IsActive);
