using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Shared;

/// <summary>عضوهای مشترک Handlerهای این بخش (قبلاً در CompanyBalanceHandlers).</summary>
public abstract class CompanyBalanceHandlerBase(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time) : FacilityPanelHandlerBase(db, user, time)
{
    public const string RestoreBalances = "balances";

    protected async Task<CompanyBalancesDto> BalancesAsync(Company company, CancellationToken ct)
    {
        var settings = await SettingsAsync(company.SchemeId, ct);
        var credit = CreditRules.CreditByMonth(settings, company, PlansOf(company, settings), await AllocationsAsync(company.Id, ct), Today);
        var docCounts = await Db.BalanceDocuments.Where(d => d.CompanyId == company.Id && d.Month != null)
            .GroupBy(d => d.Month!).Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count, ct);
        var months = Jalali.ContractMonths(company.ContractDate, Today).Select(m =>
        {
            var row = company.Balances.FirstOrDefault(b => b.Month == m);
            var docs = docCounts.GetValueOrDefault(m);
            var (remaining, used) = credit[m];
            return new MonthlyBalanceDto(m, row?.Amount, row != null ? remaining : null, docs, docs > 0, row?.EditedBy.ToDto(), row?.EditedAt, used);
        }).ToList();
        return new CompanyBalancesDto(company.Id, months, company.BalancesVersion.ToString());
    }

    protected async Task<IReadOnlyList<BalanceDocumentDto>> DocumentsAsync(long companyId, string? month, CancellationToken ct)
    {
        var docs = await Db.BalanceDocuments.Include(d => d.File)
            .Where(d => d.CompanyId == companyId && (month == null || d.Month == month)).OrderByDescending(d => d.Id).ToListAsync(ct);
        var typeIds = docs.Select(d => d.DocumentTypeId).Distinct().ToList();
        var types = await Db.Reasons.Where(r => typeIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id, r => r.Title, ct);
        return docs.Select(d => new BalanceDocumentDto(d.Id, d.Month, types.GetValueOrDefault(d.DocumentTypeId, ""), d.Description, d.File.ToDto())).ToList();
    }
}
