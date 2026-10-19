using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Commands.UpdateCompanyBalances;

/// <summary>
/// ذخیرهٔ میانگین‌ها، همراه با مستندش — در یک تراکنش.
///
/// چرا یکی شد: قبلاً ذخیرهٔ میانگین و الصاق مستند دو سرویس جدا بودند و هیچ الزامی به مستند
/// نبود، پس اگر ذخیره موفق و بارگذاری ناموفق می‌شد، میانگین بدون مستند می‌ماند. طراحی
/// می‌گوید هر تغییر میانگین باید مستند داشته باشد؛ با یک سرویس، یا هر دو ثبت می‌شوند یا هیچ.
///
/// مستند فقط وقتی لازم است که مبلغی واقعاً عوض شود. یک مستند همهٔ ماه‌هایی را که در همین
/// ذخیره تغییر کرده‌اند پوشش می‌دهد (همان رفتاری که فرانت دارد): فایل یک بار ذخیره می‌شود و
/// به هر ماه تغییریافته وصل می‌شود، پس «مستندات» هر ماه همان فایل را نشان می‌دهد.
///
/// بدنه multipart است (فایل دارد).
/// </summary>
public sealed record UpdateCompanyBalancesCommand : FacilityBaseCommand<FacilityBaseMessageResponse<CompanyBalancesDto>>
{
    [JsonIgnore, BindNever] public long CompanyId { get; init; }
    // = new List — نه [] : آرایهٔ ثابت‌اندازه جلوی Model Binding فرم (multipart) را می‌گیرد و فهرست خالی می‌ماند.
    [Required, MinLength(1), MaxLength(24)] public IReadOnlyList<MonthlyBalanceInput> Months { get; init; } = new List<MonthlyBalanceInput>();
    [Required] public string RowVersion { get; init; } = null!;

    /// <summary>نوع مستند از فهرست همین طرح (ReasonType.DocumentType). اگر مبلغی عوض شود الزامی است.</summary>
    [Range(1, int.MaxValue)] public int? DocumentTypeId { get; init; }

    /// <summary>توضیح مستند، تا ۵۰۰ حرف.</summary>
    [StringLength(500)] public string? DocumentDescription { get; init; }

    /// <summary>یک یا چند فایل؛ pdf, doc, docx, xls, xlsx, zip, jpg, png تا ۵ مگابایت هر کدام.</summary>
    [JsonIgnore, BindNever] public IReadOnlyList<FileUpload> Files { get; init; } = [];
}
