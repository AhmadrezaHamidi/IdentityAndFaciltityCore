using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Employees.Queries.GetPersonnelImportTemplate;

public sealed class GetPersonnelImportTemplateQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : PersonnelHandlerBase(db, user, time), IRequestHandler<GetPersonnelImportTemplateQuery, FacilityBaseMessageResponse<FileUpload>>
{
    public Task<FacilityBaseMessageResponse<FileUpload>> Handle(GetPersonnelImportTemplateQuery request, CancellationToken ct) =>
        Task.FromResult(Ok(new FileUpload(PersonnelSheetReader.TemplateCsv(), "personnel-sample.csv", "text/csv")));
}
