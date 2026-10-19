using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Employees.Commands.UpdatePersonnel;

public sealed class UpdatePersonnelCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : PersonnelHandlerBase(db, user, time), IRequestHandler<UpdatePersonnelCommand, FacilityBaseMessageResponse<PersonnelDetailsDto>>
{
    public async Task<FacilityBaseMessageResponse<PersonnelDetailsDto>> Handle(UpdatePersonnelCommand request, CancellationToken ct)
    {
        var (person, company) = await PersonAsync(request.Id, ct, K.PersonnelEdit);
        person.EnsureRowVersion(request.RowVersion);
        var data = new PersonnelData(request.FullName, request.NationalCode, request.Mobile, request.PersonnelCode, request.HireDate, request.Priority).Normalize(Today);
        await EnsureUniqueAsync(company.Id, data.NationalCode, person.Id, ct);
        var before = new { person.FullName, person.NationalCode, person.Mobile, person.PersonnelCode, person.HireDate, person.Priority, person.Status };
        person.Edit(data, await IsLockedAsync(person.Id, ct), Me, Now);
        Log(ChangeLogCategory.Personnel, before.Status == PersonnelStatus.Rejected ? "اصلاح و ارسال دوبارهٔ پرسنل" : "ویرایش پرسنل", person.FullName, company,
            before, new { person.FullName, person.NationalCode, person.Mobile, person.PersonnelCode, person.HireDate, person.Priority, person.Status });
        await Db.SaveChangesAsync(ct);
        return Ok(person.ToDetails(false));
    }
}
