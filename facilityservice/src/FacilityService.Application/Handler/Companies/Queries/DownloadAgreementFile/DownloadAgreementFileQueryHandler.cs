using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Queries.DownloadAgreementFile;

public sealed class DownloadAgreementFileQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time, IFacilityFileStorage storage)
    : CompanyHandlerBase(db, user, time), IRequestHandler<DownloadAgreementFileQuery, FacilityBaseMessageResponse<FileUpload>>
{
    public async Task<FacilityBaseMessageResponse<FileUpload>> Handle(DownloadAgreementFileQuery request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.CompanyId, ct, K.CompaniesView);
        if (company.AgreementFile == null) throw FacilityDomainException.NotFound("فایل تفاهم‌نامه بارگذاری نشده است.");
        return Ok(await ReadAsync(storage, company.AgreementFile, ct));
    }
}
