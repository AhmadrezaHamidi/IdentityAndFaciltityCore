using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Commands.CancelCompanyContract;

public sealed class CancelCompanyContractCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time, IFacilityFileStorage storage)
    : CompanyHandlerBase(db, user, time), IRequestHandler<CancelCompanyContractCommand, FacilityBaseMessageResponse<CompanyDetailsDto>>
{
    public async Task<FacilityBaseMessageResponse<CompanyDetailsDto>> Handle(CancelCompanyContractCommand request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.Id, ct, K.CompaniesView, K.CompaniesContractCancel);
        Require(K.CompaniesContractCancel, company, "به لغو قرارداد این شرکت دسترسی ندارید.");
        var reason = await ReasonAsync(request.ReasonId, ReasonType.ContractCancel, "انتخاب دلیل لغو الزامی است.", ct);
        var letter = await StoreAsync(storage, request.LetterFile, FileRules.LetterExtensions, FileRules.FiveMb, required: true, ct);
        company.Cancel(request.CancelDate, reason, request.Description, letter!, Me, Now, Today);
        Log(ChangeLogCategory.Company, "لغو قرارداد", $"لغو قرارداد شرکت: {company.Name} — {reason.Title}", company,
            new { status = CompanyStatus.Active }, new { status = CompanyStatus.Cancelled });
        await Db.SaveChangesAsync(ct);
        return Ok(await DetailsAsync(company, ct));
    }
}
