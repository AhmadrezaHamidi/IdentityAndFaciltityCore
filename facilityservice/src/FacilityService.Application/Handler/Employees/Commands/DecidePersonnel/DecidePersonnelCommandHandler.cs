using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Employees.Commands.DecidePersonnel;

public sealed class DecidePersonnelCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : PersonnelHandlerBase(db, user, time), IRequestHandler<DecidePersonnelCommand, FacilityBaseMessageResponse<int>>
{
    public async Task<FacilityBaseMessageResponse<int>> Handle(DecidePersonnelCommand request, CancellationToken ct)
    {
        var approve = request.Decision == DecisionType.Approve;
        Reason? reason = null;
        // سند (پرسنل شرکت ۲): «برای رد، توضیح اختیاری ثبت شود»؛ دلیل از فهرست هم اختیاری است، ولی اگر آمد باید معتبر باشد.
        // دلیل‌ها به طرح تعلق دارند و دسته می‌تواند چند شرکت از چند طرح داشته باشد، پس
        // تطابق طرح برای هر نفر جدا بررسی می‌شود.
        if (!approve && request.ReasonId is > 0)
            reason = await ReasonAnySchemeAsync(request.ReasonId, ReasonType.PersonnelReject, "دلیل رد انتخاب‌شده معتبر نیست.", ct);

        var ids = request.PersonnelIds.Distinct().ToList();
        var people = await Db.Personnel.Include(p => p.History).Where(p => ids.Contains(p.Id)).ToListAsync(ct);
        var companyIds = people.Select(p => p.CompanyId).Distinct().ToList();
        var companies = await Db.Companies.Where(c => companyIds.Contains(c.Id) && c.DeletedAt == null).ToDictionaryAsync(c => c.Id, ct);
        var processed = 0;
        foreach (var person in people)
        {
            if (!companies.TryGetValue(person.CompanyId, out var company) || !CanOn(K.PersonnelApprove, company)) continue;
            if (reason != null && reason.SchemeId != company.SchemeId) continue;
            var reasonTitle = reason?.Title;
            if (!person.TryDecide(approve, reasonTitle, request.Description?.Trim(), Me, Now)) continue;
            processed++;
            Log(ChangeLogCategory.Personnel, approve ? "تأیید پرسنل" : "رد پرسنل", person.FullName + (reasonTitle != null ? $" — {reasonTitle}" : ""), company);
        }
        await Db.SaveChangesAsync(ct);
        return Ok(processed, $"{PersianText.Number(processed)} نفر {(approve ? "تأیید" : "رد")} شد.");
    }
}
