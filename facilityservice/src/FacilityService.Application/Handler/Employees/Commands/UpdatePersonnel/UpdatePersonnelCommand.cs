using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Employees.Commands.UpdatePersonnel;

/// <summary>ویرایش؛ برای فرد ردشده یعنی «اصلاح و ارسال دوباره» (وضعیت ← در انتظار تأیید مدیر).</summary>
public sealed record UpdatePersonnelCommand : FacilityBaseCommand<FacilityBaseMessageResponse<PersonnelDetailsDto>>
{
    [JsonIgnore, BindNever] public long Id { get; init; }
    [Required, StringLength(60, MinimumLength = 3), RegularExpression(@"^[؀-ۿ‌\s]+$")] public string FullName { get; init; } = null!;
    [Required, RegularExpression(@"^\d{10}$")] public string NationalCode { get; init; } = null!;
    [Required, RegularExpression(@"^09\d{9}$")] public string Mobile { get; init; } = null!;
    [Required, StringLength(20, MinimumLength = 1), RegularExpression(@"^[a-zA-Z0-9\-_]+$")] public string PersonnelCode { get; init; } = null!;
    [Required] public DateOnly HireDate { get; init; }
    [Range(1, 999)] public int Priority { get; init; } = 1;
    [Required] public string RowVersion { get; init; } = null!;
}
