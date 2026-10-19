using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Approvals.CommandResponses;

/// <param name="Skipped">درخواست‌هایی که در این مرحله نبودند یا خارج از دامنهٔ کاربر بودند (با دلیل)</param>
public sealed record DecisionResultDto(int Processed, IReadOnlyList<SkippedRequestDto> Skipped);
