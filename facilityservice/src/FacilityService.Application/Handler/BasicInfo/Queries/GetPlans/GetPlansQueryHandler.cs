using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Queries.GetPlans;

public sealed class GetPlansQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : FacilityPanelHandlerBase(db, user, time), IRequestHandler<GetPlansQuery, FacilityBaseMessageResponse<PlanListDto>>
{
    public async Task<FacilityBaseMessageResponse<PlanListDto>> Handle(GetPlansQuery request, CancellationToken ct)
    {
        var settings = await SettingsAsync(request.SchemeId, ct);
        var plans = settings.Plans.Where(p => p.IsActive)
            .OrderBy(p => p.ContractType).ThenBy(p => p.TermMonths).ThenBy(p => p.Rate).Select(p => p.ToDto()).ToList();
        return Ok(new PlanListDto(settings.MaxPlansPerCompany, plans));
    }
}
