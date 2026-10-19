using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Employees.Shared;

/// <summary>عضوهای مشترک Handlerهای این بخش (قبلاً در PersonnelHandlers).</summary>
public abstract class PersonnelHandlerBase(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time) : FacilityPanelHandlerBase(db, user, time)
{
    public const string RestorePersonnelDelete = "personnel-delete";

    // ── کمکی ──

    protected bool SeesUnapproved(Company company) => CanOnAny(company, K.PersonnelCreate, K.PersonnelApprove, K.PersonnelEdit);

    protected async Task<(Personnel, Company)> PersonAsync(long id, CancellationToken ct, params string[] keys)
    {
        var person = await Db.Personnel.Include(p => p.History).FirstOrDefaultAsync(p => p.Id == id && p.Status != PersonnelStatus.Deleted, ct)
                     ?? throw FacilityDomainException.NotFound("پرسنل یافت نشد.");
        try
        {
            return (person, await CompanyInScopeAsync(person.CompanyId, ct, keys));
        }
        catch (FacilityDomainException ex) when (ex.StatusCode == 404)
        {
            throw FacilityDomainException.NotFound("پرسنل یافت نشد.");
        }
    }

    protected static void EnsureActive(Company company)
    {
        if (company.Status != CompanyStatus.Active) throw new FacilityDomainException("قرارداد این شرکت فعال نیست.");
    }

    protected async Task EnsureUniqueAsync(long companyId, string nationalCode, long? exceptId, CancellationToken ct)
    {
        if (await Db.Personnel.AnyAsync(p => p.CompanyId == companyId && p.Id != exceptId && p.Status != PersonnelStatus.Deleted && p.NationalCode == nationalCode, ct))
            throw Invalid("این کد ملی قبلاً در این شرکت ثبت شده است.");
    }

    /// <summary>فردی که در درخواست وام (هر وضعیتی جز لغو) استفاده شده ← ویرایش و حذف مسدود.</summary>
    protected async Task<HashSet<long>> LockedAsync(IReadOnlyList<long> ids, CancellationToken ct) =>
        (await Db.LoanRequests.Where(l => ids.Contains(l.PersonnelId) && l.Status != LoanRequestStatus.Cancelled)
            .Select(l => l.PersonnelId).Distinct().ToListAsync(ct)).ToHashSet();

    protected Task<bool> IsLockedAsync(long id, CancellationToken ct) =>
        Db.LoanRequests.AnyAsync(l => l.PersonnelId == id && l.Status != LoanRequestStatus.Cancelled, ct);
}
