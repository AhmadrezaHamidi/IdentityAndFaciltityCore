using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Employees.Queries.GetPersonnel;

public sealed class GetPersonnelQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : PersonnelHandlerBase(db, user, time), IRequestHandler<GetPersonnelQuery, FacilityBaseMessageResponse<PersonnelDetailsDto>>
{
    public async Task<FacilityBaseMessageResponse<PersonnelDetailsDto>> Handle(GetPersonnelQuery request, CancellationToken ct)
    {
        var (person, company) = await PersonAsync(request.Id, ct, K.PersonnelView);
        if (person.Status != PersonnelStatus.Approved && !SeesUnapproved(company)) throw FacilityDomainException.NotFound("پرسنل یافت نشد.");
        return Ok(person.ToDetails(await IsLockedAsync(person.Id, ct)));
    }
}
