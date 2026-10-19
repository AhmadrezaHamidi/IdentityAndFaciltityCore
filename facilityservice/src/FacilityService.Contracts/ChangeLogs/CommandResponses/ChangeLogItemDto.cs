using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.ChangeLogs.CommandResponses;

/// <param name="CanRestore">فقط ادمین و فقط عملیات قابل بازگردانی</param>
public sealed record ChangeLogItemDto(
    long Id, DateTime At, RegistrarDto User, ChangeLogCategory Category, string Action, string Details,
    string? Before, string? After, long? CompanyId, bool CanRestore, DateTime? RestoredAt);
