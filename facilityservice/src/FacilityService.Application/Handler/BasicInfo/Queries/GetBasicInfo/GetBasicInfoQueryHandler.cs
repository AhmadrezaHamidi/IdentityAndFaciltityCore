using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Queries.GetBasicInfo;

public sealed class GetBasicInfoQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : FacilityPanelHandlerBase(db, user, time), IRequestHandler<GetBasicInfoQuery, FacilityBaseMessageResponse<BasicInfoDto>>
{
    public async Task<FacilityBaseMessageResponse<BasicInfoDto>> Handle(GetBasicInfoQuery request, CancellationToken ct) =>
        Ok((await SettingsAsync(request.SchemeId, ct)).ToDto());
}
