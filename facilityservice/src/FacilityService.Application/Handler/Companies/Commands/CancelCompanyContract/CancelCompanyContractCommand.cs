using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Companies.Commands.CancelCompanyContract;

/// <summary>مدال لغو قرارداد (US-B6): تاریخ، دلیل از فهرست، توضیح و فایل نامه همه الزامی.</summary>
public sealed record CancelCompanyContractCommand : FacilityBaseCommand<FacilityBaseMessageResponse<CompanyDetailsDto>>
{
    [JsonIgnore, BindNever] public long Id { get; init; }

    /// <summary>فیلد ۲۳۵: از ۱۴۰۵/۰۱/۰۱ تا امروز</summary>
    [Required] public DateOnly CancelDate { get; init; }

    /// <summary>فیلد ۲۳۶: از «دلایل لغو قرارداد» اطلاعات پایه</summary>
    [Required, Range(1, int.MaxValue)] public int ReasonId { get; init; }

    /// <summary>فیلد ۲۳۷: ۵ تا ۱۰۰۰ حرف</summary>
    [Required, StringLength(1000, MinimumLength = 5)] public string Description { get; init; } = null!;

    /// <summary>فایل نامهٔ لغو (pdf, jpg, jpeg, png تا ۵ مگابایت) — الزامی</summary>
    [JsonIgnore, BindNever] public FileUpload? LetterFile { get; init; }
}
