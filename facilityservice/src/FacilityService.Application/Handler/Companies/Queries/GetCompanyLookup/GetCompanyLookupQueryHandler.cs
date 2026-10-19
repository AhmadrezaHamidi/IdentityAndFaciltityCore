using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Queries.GetCompanyLookup;

public sealed class GetCompanyLookupQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : CompanyHandlerBase(db, user, time), IRequestHandler<GetCompanyLookupQuery, FacilityBaseMessageResponse<IReadOnlyList<FacilityLookupItemDto>>>
{
    public async Task<FacilityBaseMessageResponse<IReadOnlyList<FacilityLookupItemDto>>> Handle(GetCompanyLookupQuery request, CancellationToken ct)
    {
        var term = request.Term?.Trim();
        var found = new Dictionary<long, string>();
        foreach (var key in LookupKeys.Where(Permissions.HasAnywhere))
        {
            var q = ScopedCompanies(key);
            if (request.ActiveOnly) q = q.Where(c => c.Status == CompanyStatus.Active);
            if (!string.IsNullOrEmpty(term)) q = q.Where(c => c.Name.Contains(term));
            foreach (var c in await q.OrderBy(c => c.Name).Take(100).Select(c => new { c.Id, c.Name }).ToListAsync(ct))
                found.TryAdd(c.Id, c.Name);
        }
        return Ok<IReadOnlyList<FacilityLookupItemDto>>(found.OrderBy(x => x.Value).Select(x => new FacilityLookupItemDto(x.Key.ToString(), x.Value)).ToList());
    }
}
