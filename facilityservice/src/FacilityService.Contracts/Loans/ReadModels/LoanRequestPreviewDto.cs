using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Loans.ReadModels;

/// <param name="Messages">پیام‌های هینت به ترتیب نمایش (سبز/قرمز/راهنما)</param>
public sealed record LoanRequestPreviewDto(
    int SelectedCount, long Total, long RemainingCredit, bool IsAllowed, int? MaxCoverablePeople,
    long? SuggestedMaxAmount, IReadOnlyList<HintDto> Messages);
