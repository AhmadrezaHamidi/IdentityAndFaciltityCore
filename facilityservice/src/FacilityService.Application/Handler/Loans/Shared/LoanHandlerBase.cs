using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Loans.Shared;

/// <summary>عضوهای مشترک Handlerهای این بخش (قبلاً در LoanHandlers).</summary>
public abstract class LoanHandlerBase(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time) : FacilityPanelHandlerBase(db, user, time)
{
    /// <summary>فقط پرسنل تأییدشدهٔ همین شرکت که درخواست باز ندارند.</summary>
    protected async Task<Dictionary<long, Personnel>> EligiblePersonnelAsync(long companyId, IReadOnlyList<long> ids, CancellationToken ct)
    {
        var people = await Db.Personnel.Where(p => ids.Contains(p.Id) && p.CompanyId == companyId).ToDictionaryAsync(p => p.Id, ct);
        if (ids.Any(id => !people.TryGetValue(id, out var p) || p.Status != PersonnelStatus.Approved))
            throw Invalid("فقط پرسنل تأییدشدهٔ همین شرکت قابل انتخاب است.");
        var open = await Db.LoanRequests.Where(l => ids.Contains(l.PersonnelId) && LoanRequest.OpenStatuses.Contains(l.Status))
            .Select(l => l.PersonnelId).FirstOrDefaultAsync(ct);
        if (open != 0) throw Invalid($"برای «{people[open].FullName}» درخواست باز وجود دارد.");
        return people;
    }

    protected async Task<LoanBatchDto> BatchDtoAsync(LoanBatch batch, CancellationToken ct) =>
        new(batch.Id, batch.CompanyId, batch.PlanId, batch.Mode,
            batch.Requests.Where(r => r.Status != LoanRequestStatus.Cancelled).Sum(r => r.Amount),
            await LoanItemsAsync(batch.Requests, ct));
}
