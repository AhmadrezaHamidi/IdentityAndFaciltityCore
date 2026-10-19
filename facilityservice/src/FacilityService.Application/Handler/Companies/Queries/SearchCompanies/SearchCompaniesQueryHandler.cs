using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Companies.Queries.SearchCompanies;

public sealed class SearchCompaniesQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : CompanyHandlerBase(db, user, time), IRequestHandler<SearchCompaniesQuery, FacilityBaseMessageResponse<CompanySearchResultDto>>
{
    public async Task<FacilityBaseMessageResponse<CompanySearchResultDto>> Handle(SearchCompaniesQuery request, CancellationToken ct)
    {
        var scoped = ScopedCompanies(K.CompaniesView);
        var activeCount = await scoped.CountAsync(c => c.Status == CompanyStatus.Active, ct);
        var registeredCount = await scoped.CountAsync(ct);

        var query = scoped;
        var term = request.Term?.Trim();
        if (!string.IsNullOrEmpty(term))
            query = query.Where(c => c.Name.Contains(term) || c.NationalId.Contains(term) || c.AgreementNumber.Contains(term));
        if (request.Status != null) query = query.Where(c => c.Status == request.Status);
        // فیلتر شعبه برای ادمین/دیجیتال؛ برای کاربر شعبه دامنه از قبل اعمال شده و این فقط محدودتر می‌کند
        if (!string.IsNullOrWhiteSpace(request.BranchCode)) query = query.Where(c => c.BranchCode == request.BranchCode);

        var (page, size) = Paging(request.Page, request.PageSize);
        var total = await query.CountAsync(ct);
        var items = await query.Include(c => c.Plans).Include(c => c.Cancellation).OrderByDescending(c => c.Id).Skip((page - 1) * size).Take(size).ToListAsync(ct);
        return Ok(new CompanySearchResultDto(Page(items.Select(c => c.ToListItem()).ToList(), total, page, size), activeCount, registeredCount));
    }
}
