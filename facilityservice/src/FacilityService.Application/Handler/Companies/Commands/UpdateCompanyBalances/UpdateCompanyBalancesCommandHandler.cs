using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Commands.UpdateCompanyBalances;

public sealed class UpdateCompanyBalancesCommandHandler(
    IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time, IFacilityFileStorage storage)
    : CompanyBalanceHandlerBase(db, user, time), IRequestHandler<UpdateCompanyBalancesCommand, FacilityBaseMessageResponse<CompanyBalancesDto>>
{
    public async Task<FacilityBaseMessageResponse<CompanyBalancesDto>> Handle(UpdateCompanyBalancesCommand request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.CompanyId, ct, K.CompaniesView, K.CompaniesBalanceEdit);
        Require(K.CompaniesBalanceEdit, company, "فقط کاربر شعبهٔ همین شرکت و ادمین می‌توانند میانگین ثبت کنند.");
        company.EnsureBalancesVersion(request.RowVersion);
        var snapshot = company.BalanceSnapshot();
        var changes = company.SetBalances(request.Months.Select(m => (m.Month, m.AverageBalance)).ToList(), Me, Now, Today);
        if (changes.Count > 0)
        {
            await AttachDocumentAsync(company, changes, request, ct);

            var details = string.Join("، ", changes.Select(c =>
                $"{PersianText.Digits(c.Month)}: {(c.Before is { } b ? PersianText.Number(b) : "—")} ← {(c.After is { } a ? PersianText.Number(a) : "—")}"));
            Log(ChangeLogCategory.Company, "ثبت میانگین موجودی", $"{company.Name}: {details}", company,
                snapshot, company.BalanceSnapshot(), RestoreBalances, new BalancesRestorePayload(company.Id, snapshot));
        }
        await Db.SaveChangesAsync(ct);
        return Ok(await BalancesAsync(company, ct));
    }

    /// <summary>
    /// مستند اجباری تغییر میانگین. فایل یک بار ذخیره می‌شود و برای هر ماه تغییریافته یک ردیف
    /// مستند به همان فایل اشاره می‌کند، پس «مستندات» هر ماه همان فایل را نشان می‌دهد و
    /// GET Balances/Documents?month= بی‌تغییر کار می‌کند.
    /// </summary>
    private async Task AttachDocumentAsync(Company company, IReadOnlyList<(string Month, long? Before, long? After)> changes,
        UpdateCompanyBalancesCommand request, CancellationToken ct)
    {
        Require(K.CompaniesDocumentsUpload, company, "ثبت میانگین مستند لازم دارد و شما به الصاق مستند دسترسی ندارید.");
        if (request.Files.Count == 0) throw Invalid("برای تغییر میانگین موجودی، الصاق مستند الزامی است.");
        await ReasonAsync(request.DocumentTypeId, ReasonType.DocumentType, company.SchemeId, "نوع مستند را انتخاب کنید.", ct);

        // همهٔ فایل‌ها قبل از ذخیرهٔ اولی اعتبارسنجی می‌شوند، تا فایل نیمه‌ذخیره نماند
        foreach (var f in request.Files)
        {
            var error = FileRules.Validate(f, FileRules.BalanceDocumentExtensions, FileRules.FiveMb, required: true);
            if (error != null) throw Invalid($"«{f.FileName}»: {error}");
        }

        var description = request.DocumentDescription?.Trim();
        foreach (var f in request.Files)
        {
            var stored = await StoreAsync(storage, f, FileRules.BalanceDocumentExtensions, FileRules.FiveMb, required: true, ct);
            foreach (var change in changes)
                Db.BalanceDocuments.Add(new BalanceDocument(company.Id, change.Month, request.DocumentTypeId!.Value, description, stored!));
        }
    }
}
