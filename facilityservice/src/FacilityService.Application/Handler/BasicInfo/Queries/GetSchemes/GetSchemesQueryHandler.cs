using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Queries.GetSchemes;

public sealed class GetSchemesQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : FacilityPanelHandlerBase(db, user, time), IRequestHandler<GetSchemesQuery, FacilityBaseMessageResponse<IReadOnlyList<SchemeDto>>>
{
    public async Task<FacilityBaseMessageResponse<IReadOnlyList<SchemeDto>>> Handle(GetSchemesQuery request, CancellationToken ct) =>
        Ok<IReadOnlyList<SchemeDto>>(await Db.Schemes.OrderBy(s => s.Id).Select(s => new SchemeDto(s.Id, s.Code, s.Title, s.IsActive)).ToListAsync(ct));
}
