using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Employees.Commands.CreatePersonnel;

public sealed record CreatePersonnelCommand : FacilityBaseCommand<FacilityBaseMessageResponse<PersonnelDetailsDto>>
{
    [JsonIgnore, BindNever] public long CompanyId { get; init; }

    /// <summary>فیلد ۲۴۱: ۳ تا ۶۰ حرف فارسی</summary>
    [Required, StringLength(60, MinimumLength = 3), RegularExpression(@"^[؀-ۿ‌\s]+$")]
    public string FullName { get; init; } = null!;

    /// <summary>فیلد ۲۴۲: کد ملی ۱۰ رقم</summary>
    [Required, RegularExpression(@"^\d{10}$")] public string NationalCode { get; init; } = null!;

    /// <summary>فیلد ۲۴۳: موبایل ۱۱ رقم با ۰۹</summary>
    [Required, RegularExpression(@"^09\d{9}$")] public string Mobile { get; init; } = null!;

    /// <summary>فیلد ۲۴۴: کد پرسنلی ۱ تا ۲۰؛ حرف، عدد، - _</summary>
    [Required, StringLength(20, MinimumLength = 1), RegularExpression(@"^[a-zA-Z0-9\-_]+$")]
    public string PersonnelCode { get; init; } = null!;

    /// <summary>فیلد ۲۴۵: ۱۳۵۰/۰۱/۰۱ تا امروز (تاریخ آینده ممنوع)</summary>
    [Required] public DateOnly HireDate { get; init; }

    /// <summary>فیلد ۲۴۶: ۱ تا ۹۹۹</summary>
    [Range(1, 999)] public int Priority { get; init; } = 1;
}
