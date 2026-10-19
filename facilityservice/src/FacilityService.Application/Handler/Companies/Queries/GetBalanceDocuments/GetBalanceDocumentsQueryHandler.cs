using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Queries.GetBalanceDocuments;

public sealed class GetBalanceDocumentsQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : CompanyBalanceHandlerBase(db, user, time), IRequestHandler<GetBalanceDocumentsQuery, FacilityBaseMessageResponse<IReadOnlyList<BalanceDocumentDto>>>
{
    public async Task<FacilityBaseMessageResponse<IReadOnlyList<BalanceDocumentDto>>> Handle(GetBalanceDocumentsQuery request, CancellationToken ct)
    {
        await CompanyInScopeAsync(request.CompanyId, ct, K.CompaniesDocumentsView);
        return Ok(await DocumentsAsync(request.CompanyId, request.Month, ct));
    }
}
