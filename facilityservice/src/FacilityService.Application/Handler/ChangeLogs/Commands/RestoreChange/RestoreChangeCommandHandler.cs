using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.ChangeLogs.Commands.RestoreChange;

public sealed class RestoreChangeCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : ChangeLogHandlerBase(db, user, time), IRequestHandler<RestoreChangeCommand, FacilityBaseMessageResponse<ChangeLogItemDto>>
{
    public async Task<FacilityBaseMessageResponse<ChangeLogItemDto>> Handle(RestoreChangeCommand request, CancellationToken ct)
    {
        if (!RestoreEnabled) throw Invalid("بازگردانی فعلاً فعال نیست.");
        var entry = await Scoped().FirstOrDefaultAsync(l => l.Id == request.Id, ct) ?? throw FacilityDomainException.NotFound("رکورد یافت نشد.");
        if (!entry.CanRestore) throw Invalid("این عملیات قابل بازگردانی نیست.");
        Company? company = null;
        switch (entry.RestoreKind)
        {
            case CompanyBalanceHandlerBase.RestoreBalances:
            {
                var payload = JsonSerializer.Deserialize<BalancesRestorePayload>(entry.RestorePayload!, Json)!;
                company = await CompanyInScopeAsync(payload.CompanyId, ct, K.AuditRestore);
                company.RestoreBalances(payload.Balances, Me, Now);
                break;
            }
            case PersonnelHandlerBase.RestorePersonnelDelete:
            {
                var payload = JsonSerializer.Deserialize<PersonnelRestorePayload>(entry.RestorePayload!, Json)!;
                var person = await Db.Personnel.Include(p => p.History).FirstOrDefaultAsync(p => p.Id == payload.PersonnelId, ct)
                             ?? throw FacilityDomainException.NotFound("پرسنل یافت نشد.");
                company = await CompanyInScopeAsync(person.CompanyId, ct, K.AuditRestore);
                if (await Db.Personnel.AnyAsync(p => p.CompanyId == person.CompanyId && p.Id != person.Id && p.Status != PersonnelStatus.Deleted && p.NationalCode == person.NationalCode, ct))
                    throw Invalid("فرد دیگری با همین کد ملی در شرکت ثبت شده و بازگردانی ممکن نیست.");
                person.Undelete(payload.Status, Me, Now);
                break;
            }
            default:
                throw Invalid("این عملیات قابل بازگردانی نیست.");
        }
        entry.MarkRestored(Now);
        Log(entry.Category, "بازگردانی", $"{entry.Action} — {request.Description.Trim()}", company);
        await Db.SaveChangesAsync(ct);
        return Ok(ToDto(entry, true));
    }
}
