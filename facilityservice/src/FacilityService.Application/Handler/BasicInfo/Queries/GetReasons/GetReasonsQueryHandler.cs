using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Queries.GetReasons;

public sealed class GetReasonsQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : FacilityPanelHandlerBase(db, user, time), IRequestHandler<GetReasonsQuery, FacilityBaseMessageResponse<IReadOnlyList<ReasonDto>>>
{
    public async Task<FacilityBaseMessageResponse<IReadOnlyList<ReasonDto>>> Handle(GetReasonsQuery request, CancellationToken ct)
    {
        // فهرست غیرفعال‌ها فقط برای کسی که اطلاعات پایه را ویرایش می‌کند
        var includeInactive = request.IncludeInactive && Permissions.HasAnywhere(FacilityPermissionKeys.BasicInfoEdit);
        return Ok<IReadOnlyList<ReasonDto>>(await Db.Reasons
            .Where(r => r.Type == request.Type && (includeInactive || r.IsActive))
            .OrderBy(r => r.SortOrder).ThenBy(r => r.Id)
            .Select(r => new ReasonDto(r.Id, r.Type, r.Title, r.IsActive, r.SortOrder)).ToListAsync(ct));
    }
}
