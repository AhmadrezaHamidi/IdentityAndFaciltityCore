using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Commands.DeleteCompany;

public sealed class DeleteCompanyCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : CompanyHandlerBase(db, user, time), IRequestHandler<DeleteCompanyCommand, FacilityBaseMessageResponse<bool>>
{
    public async Task<FacilityBaseMessageResponse<bool>> Handle(DeleteCompanyCommand request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.Id, ct, K.CompaniesDelete);
        if (await Db.LoanRequests.AnyAsync(l => l.CompanyId == company.Id && l.Status == LoanRequestStatus.Approved, ct))
            throw Invalid("شرکتی که درخواست تأییدشده دارد حذف نمی‌شود؛ قرارداد را لغو کنید.");
        if (await Db.LoanRequests.AnyAsync(l => l.CompanyId == company.Id && LoanRequest.OpenStatuses.Contains(l.Status), ct))
            throw Invalid("این شرکت درخواست وام در جریان تأیید دارد؛ ابتدا آن درخواست‌ها را لغو یا تعیین وضعیت کنید.");
        company.Delete(request.Description, Now);
        Log(ChangeLogCategory.Company, "حذف شرکت", $"{company.Name} — {request.Description.Trim()}", company);
        await Db.SaveChangesAsync(ct);
        return Ok(true);
    }
}
