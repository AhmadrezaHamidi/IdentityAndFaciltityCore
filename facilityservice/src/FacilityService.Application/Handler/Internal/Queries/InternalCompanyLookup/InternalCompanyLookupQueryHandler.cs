using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Internal.Queries.InternalCompanyLookup;

public sealed class InternalCompanyLookupQueryHandler(IFacilityPanelDbContext db)
    : IRequestHandler<InternalCompanyLookupQuery, FacilityBaseMessageResponse<IReadOnlyList<FacilityLookupItemDto>>>
{
    public async Task<FacilityBaseMessageResponse<IReadOnlyList<FacilityLookupItemDto>>> Handle(InternalCompanyLookupQuery request, CancellationToken ct)
    {
        var q = db.Companies.Where(c => c.DeletedAt == null);
        var term = request.Term?.Trim();
        if (!string.IsNullOrEmpty(term)) q = q.Where(c => c.Name.Contains(term) || c.NationalId.Contains(term));
        var items = await q.OrderBy(c => c.Name).Take(50).Select(c => new { c.Id, c.Name }).ToListAsync(ct);
        return MessageResponse.GenerateMessageResponse<IReadOnlyList<FacilityLookupItemDto>>(200,
            items.Select(c => new FacilityLookupItemDto(c.Id.ToString(), c.Name)).ToList(), null);
    }
}
