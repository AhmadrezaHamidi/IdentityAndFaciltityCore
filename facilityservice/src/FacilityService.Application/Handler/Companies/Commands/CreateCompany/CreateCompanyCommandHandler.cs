using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Commands.CreateCompany;

public sealed class CreateCompanyCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time, IFacilityFileStorage storage)
    : CompanyHandlerBase(db, user, time), IRequestHandler<CreateCompanyCommand, FacilityBaseMessageResponse<CompanyDetailsDto>>
{
    public async Task<FacilityBaseMessageResponse<CompanyDetailsDto>> Handle(CreateCompanyCommand request, CancellationToken ct)
    {
        // ادمین/دیجیتال (دامنهٔ سراسری) می‌توانند شعبه را تعیین کنند؛ کاربر شعبه همیشه شعبهٔ خودش
        var isGlobal = Permissions.Can(K.CompaniesCreate, FacilityResource.None);
        var branchCode = (isGlobal && !string.IsNullOrWhiteSpace(request.BranchCode) ? request.BranchCode : User.BranchCode)?.Trim();
        if (string.IsNullOrEmpty(branchCode)) throw Invalid("کد شعبهٔ شرکت مشخص نیست؛ کد شعبه را وارد کنید.");
        if (!Permissions.Can(K.CompaniesCreate, new FacilityResource(branchCode, null)))
            throw FacilityDomainException.Forbidden("فقط برای شعبهٔ خودتان می‌توانید شرکت ثبت کنید.");

        if (!await Db.Schemes.AnyAsync(s => s.Id == request.SchemeId && s.IsActive, ct)) throw Invalid("طرح انتخاب‌شده معتبر نیست.");
        var settings = await SettingsAsync(request.SchemeId, ct);
        EnsurePlans(settings, request.PlanIds);
        if (await Db.Companies.AnyAsync(c => c.DeletedAt == null && c.NationalId == request.NationalId, ct))
            throw Invalid("شرکتی با این شناسهٔ ملی ثبت شده است.");

        var file = await StoreAsync(storage, request.AgreementFile, FileRules.AgreementExtensions, FileRules.FiveMb, required: false, ct);
        var company = Company.Create(request.SchemeId, request.NationalId, branchCode,
            new CompanyData(request.Name, request.AgreementNumber, request.AccountNumber, request.ContractDate, request.AccountOpeningDate,
                request.SalaryFromSepah, request.PlanIds),
            settings.MaxPlansPerCompany, Me, Now, file);
        Db.Companies.Add(company);
        Log(ChangeLogCategory.Company, "ثبت شرکت", $"ثبت شرکت جدید: {company.Name}", company);
        await Db.SaveChangesAsync(ct);
        return Ok(company.ToDetails(settings.Plans));
    }
}
