using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Employees.Queries.SearchPersonnel;

public sealed record SearchPersonnelQuery : FacilityPagedQuery<FacilityBaseMessageResponse<FacilityPagedResponse<PersonnelListItemDto>>>
{
    [JsonIgnore, BindNever] public long CompanyId { get; init; }
    public PersonnelStatus? Status { get; init; }
    public DateOnly? HireDateFrom { get; init; }
    public DateOnly? HireDateTo { get; init; }

    /// <summary>فقط پرسنل تأییدشده‌ای که هنوز درخواست وام فعال ندارند (فرم تخصیص وام).</summary>
    public bool OnlyAvailableForLoan { get; init; }
}
