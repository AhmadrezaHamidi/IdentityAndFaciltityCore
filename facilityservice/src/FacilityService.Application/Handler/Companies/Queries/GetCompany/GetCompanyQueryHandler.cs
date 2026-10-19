using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Queries.GetCompany;

public sealed class GetCompanyQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : CompanyHandlerBase(db, user, time), IRequestHandler<GetCompanyQuery, FacilityBaseMessageResponse<CompanyDetailsDto>>
{
    public async Task<FacilityBaseMessageResponse<CompanyDetailsDto>> Handle(GetCompanyQuery request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.Id, ct, K.CompaniesView);
        return Ok(await DetailsAsync(company, ct));
    }
}
