using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Employees.Commands.DeletePersonnel;

public sealed class DeletePersonnelCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time, IFacilityFileStorage storage)
    : PersonnelHandlerBase(db, user, time), IRequestHandler<DeletePersonnelCommand, FacilityBaseMessageResponse<bool>>
{
    public async Task<FacilityBaseMessageResponse<bool>> Handle(DeletePersonnelCommand request, CancellationToken ct)
    {
        var (person, company) = await PersonAsync(request.Id, ct, K.PersonnelDelete);
        var locked = await IsLockedAsync(person.Id, ct);
        if (locked) throw Invalid("این فرد در درخواست وام استفاده شده و حذف او ممکن نیست.");
        var file = await StoreAsync(storage, request.File, FileRules.LetterExtensions, FileRules.FiveMb, required: true, ct);
        var before = person.Delete(request.Description, file!, locked, Me, Now);
        Log(ChangeLogCategory.Personnel, "حذف پرسنل", $"{person.FullName} — {request.Description.Trim()}", company,
            new { status = before }, new { status = PersonnelStatus.Deleted }, RestorePersonnelDelete, new PersonnelRestorePayload(person.Id, before));
        await Db.SaveChangesAsync(ct);
        return Ok(true);
    }
}
