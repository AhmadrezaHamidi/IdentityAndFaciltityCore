using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Employees.Queries.GetPersonnelImport;

public sealed class GetPersonnelImportQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : PersonnelHandlerBase(db, user, time), IRequestHandler<GetPersonnelImportQuery, FacilityBaseMessageResponse<PersonnelImportResultDto>>
{
    public async Task<FacilityBaseMessageResponse<PersonnelImportResultDto>> Handle(GetPersonnelImportQuery request, CancellationToken ct)
    {
        var batch = await Db.PersonnelImports.FirstOrDefaultAsync(b => b.Id == request.BatchId, ct) ?? throw FacilityDomainException.NotFound("بارگذاری یافت نشد.");
        await CompanyInScopeAsync(batch.CompanyId, ct, K.PersonnelUpload);
        var errors = JsonSerializer.Deserialize<List<PersonnelImportErrorDto>>(batch.ErrorsJson, Json) ?? [];
        return Ok(new PersonnelImportResultDto(batch.Id, batch.TotalRows, batch.Accepted, batch.Rejected, errors));
    }
}
