using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Queries.SearchSchemes;

public sealed class SearchSchemesQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : FacilityPanelHandlerBase(db, user, time),
      IRequestHandler<SearchSchemesQuery, FacilityBaseMessageResponse<FacilityPagedResponse<SchemeListItemDto>>>
{
    public async Task<FacilityBaseMessageResponse<FacilityPagedResponse<SchemeListItemDto>>> Handle(
        SearchSchemesQuery request, CancellationToken ct)
    {
        var includeInactive = request.IncludeInactive && Permissions.HasAnywhere(FacilityPermissionKeys.BasicInfoEdit);
        var query = Db.Schemes.Where(s => includeInactive || s.IsActive);
        var term = request.Term?.Trim();
        if (!string.IsNullOrEmpty(term)) query = query.Where(s => s.Title.Contains(term) || s.Code.Contains(term));

        var (page, size) = Paging(request.Page, request.PageSize);
        var total = await query.CountAsync(ct);
        var schemes = await query.OrderBy(s => s.Id).Skip((page - 1) * size).Take(size).ToListAsync(ct);

        // تنظیمات و تعداد شرکت‌ها یک‌جا، نه یک پرس‌وجو به ازای هر ردیف
        var ids = schemes.Select(s => s.Id).ToList();
        var settings = await Db.SchemeSettings.Where(s => ids.Contains(s.SchemeId)).ToDictionaryAsync(s => s.SchemeId, ct);
        var companies = await Db.Companies.Where(c => ids.Contains(c.SchemeId) && c.DeletedAt == null)
            .GroupBy(c => c.SchemeId).Select(g => new { SchemeId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.SchemeId, x => x.Count, ct);

        var items = schemes.Select(s =>
        {
            // طرحی که تنظیماتش هنوز ثبت نشده، در جدول با مقدارهای خالی دیده می‌شود، نه با خطا.
            var set = settings.GetValueOrDefault(s.Id);
            return new SchemeListItemDto(s.Id, s.Code, s.Title,
                set?.QardHasanRate ?? 0,
                set?.MurabahaRates ?? [],
                set?.RepaymentTermsMonths ?? [],
                set?.ContractDurationMonths ?? 0,
                set?.MaxPlansPerCompany ?? 0,
                s.IsActive,
                companies.GetValueOrDefault(s.Id));
        }).ToList();

        return Ok(Page<SchemeListItemDto>(items, total, page, size));
    }
}
