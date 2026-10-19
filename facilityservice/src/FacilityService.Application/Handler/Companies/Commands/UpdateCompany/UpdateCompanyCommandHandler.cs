using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Commands.UpdateCompany;

public sealed class UpdateCompanyCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time, IFacilityFileStorage storage)
    : CompanyHandlerBase(db, user, time), IRequestHandler<UpdateCompanyCommand, FacilityBaseMessageResponse<CompanyDetailsDto>>
{
    public async Task<FacilityBaseMessageResponse<CompanyDetailsDto>> Handle(UpdateCompanyCommand request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.Id, ct, K.CompaniesEdit);
        company.EnsureRowVersion(request.RowVersion);
        var settings = await SettingsAsync(company.SchemeId, ct);
        EnsurePlans(settings, request.PlanIds);

        // طرحی که درخواست فعال دارد از تفاهم‌نامه حذف نمی‌شود
        var removed = company.Plans.Select(p => p.PlanId).Except(request.PlanIds).ToList();
        if (removed.Count > 0 && await Db.LoanRequests.AnyAsync(l => l.CompanyId == company.Id && removed.Contains(l.PlanId)
                && !LoanRequest.ReleasedStatuses.Contains(l.Status), ct))
            throw Invalid("طرحی که درخواست وام فعال دارد را نمی‌توان از تفاهم‌نامه حذف کرد.");

        var before = new { company.Name, company.AgreementNumber, company.AccountNumber, company.ContractDate, company.AccountOpeningDate, company.SalaryFromSepah, Plans = company.Plans.Select(p => p.PlanId).ToList() };
        var file = await StoreAsync(storage, request.AgreementFile, FileRules.AgreementExtensions, FileRules.FiveMb, required: false, ct);
        company.Update(new CompanyData(request.Name, request.AgreementNumber, request.AccountNumber, request.ContractDate,
            request.AccountOpeningDate, request.SalaryFromSepah, request.PlanIds), settings.MaxPlansPerCompany, file);
        Log(ChangeLogCategory.Company, "ویرایش شرکت", company.Name, company, before,
            new { company.Name, company.AgreementNumber, company.AccountNumber, company.ContractDate, company.AccountOpeningDate, company.SalaryFromSepah, Plans = company.Plans.Select(p => p.PlanId).ToList() });
        await Db.SaveChangesAsync(ct);
        return Ok(company.ToDetails(settings.Plans));
    }
}
