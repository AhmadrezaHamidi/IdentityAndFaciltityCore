using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Queries.DownloadBalanceDocument;

public sealed class DownloadBalanceDocumentQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time, IFacilityFileStorage storage)
    : CompanyBalanceHandlerBase(db, user, time), IRequestHandler<DownloadBalanceDocumentQuery, FacilityBaseMessageResponse<FileUpload>>
{
    public async Task<FacilityBaseMessageResponse<FileUpload>> Handle(DownloadBalanceDocumentQuery request, CancellationToken ct)
    {
        await CompanyInScopeAsync(request.CompanyId, ct, K.CompaniesDocumentsView);
        var doc = await Db.BalanceDocuments.Include(d => d.File).FirstOrDefaultAsync(d => d.Id == request.DocumentId && d.CompanyId == request.CompanyId, ct)
                  ?? throw FacilityDomainException.NotFound("مستند یافت نشد.");
        return Ok(await ReadAsync(storage, doc.File, ct));
    }
}
