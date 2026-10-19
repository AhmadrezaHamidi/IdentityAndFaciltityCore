using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Commands.UploadBalanceDocuments;

public sealed class UploadBalanceDocumentsCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time, IFacilityFileStorage storage)
    : CompanyBalanceHandlerBase(db, user, time), IRequestHandler<UploadBalanceDocumentsCommand, FacilityBaseMessageResponse<IReadOnlyList<BalanceDocumentDto>>>
{
    public async Task<FacilityBaseMessageResponse<IReadOnlyList<BalanceDocumentDto>>> Handle(UploadBalanceDocumentsCommand request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.CompanyId, ct, K.CompaniesView, K.CompaniesDocumentsUpload);
        Require(K.CompaniesDocumentsUpload, company, "به الصاق مستند برای این شرکت دسترسی ندارید.");
        if (request.Files.Count == 0) throw Invalid("حداقل یک فایل انتخاب کنید.");
        if (request.Month != null && !Jalali.ContractMonths(company.ContractDate, Today).Contains(request.Month))
            throw Invalid($"ماه {PersianText.Digits(request.Month)} در سال جاری قرارداد نیست.");
        await ReasonAsync(request.DocumentTypeId, ReasonType.DocumentType, company.SchemeId, "نوع مستند را انتخاب کنید.", ct);
        foreach (var f in request.Files)
        {
            var error = FileRules.Validate(f, FileRules.BalanceDocumentExtensions, FileRules.FiveMb, required: true);
            if (error != null) throw Invalid($"«{f.FileName}»: {error}");
        }
        foreach (var f in request.Files)
        {
            var stored = await StoreAsync(storage, f, FileRules.BalanceDocumentExtensions, FileRules.FiveMb, required: true, ct);
            Db.BalanceDocuments.Add(new BalanceDocument(company.Id, request.Month, request.DocumentTypeId, request.Description?.Trim(), stored!));
        }
        Log(ChangeLogCategory.Company, "الصاق مستند", $"{company.Name}: {string.Join("، ", request.Files.Select(f => f.FileName))}", company);
        await Db.SaveChangesAsync(ct);
        return Ok(await DocumentsAsync(company.Id, request.Month, ct));
    }
}
