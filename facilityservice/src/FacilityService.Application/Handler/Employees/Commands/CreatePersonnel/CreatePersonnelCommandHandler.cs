using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Employees.Commands.CreatePersonnel;

public sealed class CreatePersonnelCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : PersonnelHandlerBase(db, user, time), IRequestHandler<CreatePersonnelCommand, FacilityBaseMessageResponse<PersonnelDetailsDto>>
{
    public async Task<FacilityBaseMessageResponse<PersonnelDetailsDto>> Handle(CreatePersonnelCommand request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.CompanyId, ct, K.PersonnelCreate);
        EnsureActive(company);
        var data = new PersonnelData(request.FullName, request.NationalCode, request.Mobile, request.PersonnelCode, request.HireDate, request.Priority).Normalize(Today);
        await EnsureUniqueAsync(company.Id, data.NationalCode, null, ct);
        var person = Personnel.Create(company.Id, data, Me, Now);
        Db.Personnel.Add(person);
        Log(ChangeLogCategory.Personnel, "معرفی پرسنل", $"{person.FullName} ({company.Name})", company);
        await Db.SaveChangesAsync(ct);
        return Ok(person.ToDetails(false));
    }
}
