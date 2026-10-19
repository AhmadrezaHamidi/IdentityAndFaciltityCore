using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Commands.UpdateCompany;

/// <summary>ویرایش شرکت (نمونهٔ پنل). شناسهٔ ملی و شعبه قابل تغییر نیست.</summary>
public sealed record UpdateCompanyCommand : FacilityBaseCommand<FacilityBaseMessageResponse<CompanyDetailsDto>>
{
    [JsonIgnore, BindNever] public long Id { get; init; }

    [Required, StringLength(100, MinimumLength = 3)]
    [RegularExpression(@"^[؀-ۿ‌\s0-9۰-۹]+$")]
    public string Name { get; init; } = null!;

    [Required, StringLength(50, MinimumLength = 3), RegularExpression(@"^[\p{L}0-9/\-.]+$")]
    public string AgreementNumber { get; init; } = null!;

    [Required, RegularExpression(@"^\d{8,24}$")] public string AccountNumber { get; init; } = null!;
    [Required] public DateOnly ContractDate { get; init; }
    [Required] public DateOnly AccountOpeningDate { get; init; }
    public bool SalaryFromSepah { get; init; }
    // = new List — نه [] : آرایهٔ ثابت‌اندازه جلوی Model Binding فرم (multipart) را می‌گیرد و مقدار خالی می‌ماند.
    [Required, MinLength(1)] public IReadOnlyList<int> PlanIds { get; init; } = new List<int>();
    [JsonIgnore, BindNever] public FileUpload? AgreementFile { get; init; }
    [Required] public string RowVersion { get; init; } = null!;
}
