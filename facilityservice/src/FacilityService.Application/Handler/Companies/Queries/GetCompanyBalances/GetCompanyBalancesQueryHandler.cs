using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Queries.GetCompanyBalances;

public sealed class GetCompanyBalancesQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : CompanyBalanceHandlerBase(db, user, time), IRequestHandler<GetCompanyBalancesQuery, FacilityBaseMessageResponse<CompanyBalancesDto>>
{
    public async Task<FacilityBaseMessageResponse<CompanyBalancesDto>> Handle(GetCompanyBalancesQuery request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.CompanyId, ct, K.CompaniesView);
        return Ok(await BalancesAsync(company, ct));
    }
}
