using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Queries.GetScheme;

public sealed class GetSchemeQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : SchemeHandlerBase(db, user, time), IRequestHandler<GetSchemeQuery, FacilityBaseMessageResponse<SchemeFormDto>>
{
    public async Task<FacilityBaseMessageResponse<SchemeFormDto>> Handle(GetSchemeQuery request, CancellationToken ct)
    {
        var scheme = await Db.Schemes.FirstOrDefaultAsync(s => s.Id == request.SchemeId, ct)
                     ?? throw FacilityDomainException.NotFound("طرح یافت نشد.");
        return Ok(await FormAsync(scheme, ct));
    }
}
