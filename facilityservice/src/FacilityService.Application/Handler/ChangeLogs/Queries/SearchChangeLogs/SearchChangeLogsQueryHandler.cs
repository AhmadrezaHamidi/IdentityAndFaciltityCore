using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.ChangeLogs.Queries.SearchChangeLogs;

public sealed class SearchChangeLogsQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : ChangeLogHandlerBase(db, user, time), IRequestHandler<SearchChangeLogsQuery, FacilityBaseMessageResponse<FacilityPagedResponse<ChangeLogItemDto>>>
{
    public async Task<FacilityBaseMessageResponse<FacilityPagedResponse<ChangeLogItemDto>>> Handle(SearchChangeLogsQuery request, CancellationToken ct)
    {
        var query = Filter(request.Category, request.CompanyId, request.PanelUserId, request.From, request.To, request.Term);
        var (page, size) = Paging(request.Page, request.PageSize, defaultSize: 50);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(l => l.Id).Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var canRestore = Permissions.HasAnywhere(K.AuditRestore);
        return Ok(Page(items.Select(l => ToDto(l, canRestore)).ToList(), total, page, size));
    }
}
