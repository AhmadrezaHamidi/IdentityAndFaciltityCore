using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Commands.DeleteBalanceDocument;

public sealed class DeleteBalanceDocumentCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : CompanyBalanceHandlerBase(db, user, time), IRequestHandler<DeleteBalanceDocumentCommand, FacilityBaseMessageResponse<bool>>
{
    public async Task<FacilityBaseMessageResponse<bool>> Handle(DeleteBalanceDocumentCommand request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.CompanyId, ct, K.CompaniesDocumentsDelete);
        var doc = await Db.BalanceDocuments.Include(d => d.File).FirstOrDefaultAsync(d => d.Id == request.DocumentId && d.CompanyId == company.Id, ct)
                  ?? throw FacilityDomainException.NotFound("مستند یافت نشد.");
        Db.BalanceDocuments.Remove(doc);
        Log(ChangeLogCategory.Company, "حذف مستند", $"{company.Name}: {doc.File.FileName}", company);
        await Db.SaveChangesAsync(ct);
        return Ok(true);
    }
}
