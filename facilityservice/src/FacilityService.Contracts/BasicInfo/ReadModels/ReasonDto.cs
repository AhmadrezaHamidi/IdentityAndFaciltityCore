using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.BasicInfo.ReadModels;

public sealed record ReasonDto(int Id, ReasonType Type, string Title, bool IsActive, int SortOrder);
