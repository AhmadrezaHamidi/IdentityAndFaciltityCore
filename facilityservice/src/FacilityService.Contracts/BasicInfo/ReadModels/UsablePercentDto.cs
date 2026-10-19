using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.BasicInfo.ReadModels;

public sealed record UsablePercentDto(int TermMonths, ContractType ContractType, decimal Rate, decimal UsablePercent);
