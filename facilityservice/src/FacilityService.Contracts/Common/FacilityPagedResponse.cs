using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Common;

public sealed record FacilityPagedResponse<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
