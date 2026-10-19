using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Loans.ReadModels;

public sealed record LoanRequestDetailsDto(
    LoanRequestListItemDto Summary, RegistrarDto Registrar, IReadOnlyList<LoanRequestHistoryDto> History, IReadOnlyList<StoredFileDto> Documents);
