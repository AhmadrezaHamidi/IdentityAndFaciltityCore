using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Shared;

/// <summary>عضوهای مشترک Handlerهای این بخش (قبلاً در CompanyHandlers).</summary>
public abstract class CompanyHandlerBase(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time) : FacilityPanelHandlerBase(db, user, time)
{
    /// <summary>کلیدهایی که Dropdown «انتخاب شرکت» را در تب‌های مختلف باز می‌کنند.</summary>
    protected static readonly string[] LookupKeys =
        [K.CompaniesView, K.PersonnelView, K.LoansView, K.ManagerApprovalView, K.CreditApprovalView, K.DashboardView, K.AuditView];

    protected async Task<CompanyDetailsDto> DetailsAsync(Company company, CancellationToken ct) =>
        company.ToDetails(await Db.SchemePlans.Where(p => p.SchemeId == company.SchemeId).ToListAsync(ct));

    protected static void EnsurePlans(SchemeSettings settings, IReadOnlyList<int> planIds)
    {
        if (planIds.Any(id => !settings.Plans.Any(p => p.Id == id && p.IsActive)))
            throw new FacilityDomainException("طرح انتخاب‌شده در اطلاعات پایه فعال نیست.");
    }
}
