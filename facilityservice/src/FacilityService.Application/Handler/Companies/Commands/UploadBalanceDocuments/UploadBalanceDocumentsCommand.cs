using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Commands.UploadBalanceDocuments;

public sealed record UploadBalanceDocumentsCommand : FacilityBaseCommand<FacilityBaseMessageResponse<IReadOnlyList<BalanceDocumentDto>>>
{
    [JsonIgnore, BindNever] public long CompanyId { get; init; }
    [RegularExpression(@"^\d{4}/\d{2}$")] public string? Month { get; init; }

    /// <summary>فیلد ۲۶۱: نوع مستند از اطلاعات پایه (ReasonType.DocumentType)</summary>
    [Required, Range(1, int.MaxValue)] public int DocumentTypeId { get; init; }

    /// <summary>فیلد ۲۰۱: تا ۵۰۰ حرف</summary>
    [StringLength(500)] public string? Description { get; init; }

    /// <summary>یک یا چند فایل؛ pdf, doc, docx, xls, xlsx, zip, jpg, png تا ۵ مگابایت</summary>
    [JsonIgnore, BindNever] public IReadOnlyList<FileUpload> Files { get; init; } = [];
}
