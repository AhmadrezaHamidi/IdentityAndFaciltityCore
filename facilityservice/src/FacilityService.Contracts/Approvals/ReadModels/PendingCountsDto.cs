using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Approvals.ReadModels;

public sealed record PendingCountsDto(int? PersonnelApproval, int? ManagerApproval, int? CreditApproval, int? DigitalApproval);
