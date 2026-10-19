using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Companies.ReadModels;

public sealed record MonthlyBalanceInput
{
    [Required, RegularExpression(@"^\d{4}/\d{2}$")] public string Month { get; init; } = null!;

    /// <summary>فیلد ۲۲۵: ۱ ریال تا ۱۵ رقم؛ null یعنی خالی کردن</summary>
    [Range(1, 999_999_999_999_999)] public long? AverageBalance { get; init; }
}
