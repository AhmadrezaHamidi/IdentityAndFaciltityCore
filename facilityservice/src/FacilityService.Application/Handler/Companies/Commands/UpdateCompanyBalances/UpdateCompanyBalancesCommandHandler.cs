using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Commands.UpdateCompanyBalances;

public sealed class UpdateCompanyBalancesCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
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
            var details = string.Join("، ", changes.Select(c =>
                $"{PersianText.Digits(c.Month)}: {(c.Before is { } b ? PersianText.Number(b) : "—")} ← {(c.After is { } a ? PersianText.Number(a) : "—")}"));
            Log(ChangeLogCategory.Company, "ثبت میانگین موجودی", $"{company.Name}: {details}", company,
                snapshot, company.BalanceSnapshot(), RestoreBalances, new BalancesRestorePayload(company.Id, snapshot));
        }
        await Db.SaveChangesAsync(ct);
        return Ok(await BalancesAsync(company, ct));
    }
}
