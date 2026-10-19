using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Companies.ReadModels;

public sealed record CancellationInfoDto(DateOnly CancelDate, string Reason, string Description, StoredFileDto? File, RegistrarDto CancelledBy, DateTime CancelledAt);
