using IdentityService.Domain.Entity.ContentManagement;
using IdentityService.Domain.Services.ContentManagement;
using IdentityService.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories.ContentManagement;

public sealed class ContentManagementRepository : IContentManagementRepository
{
    private readonly ApplicationDBContext _db;

    public ContentManagementRepository(ApplicationDBContext db)
    {
        _db = db;
    }

    public Task<List<ContentItemEntity>> ListAsync(string category, CancellationToken cancellationToken)
        => _db.ContentItem.AsNoTracking()
            .Where(i => i.Category == category)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync(cancellationToken);

    public Task<ContentItemEntity?> GetAsync(string category, int id, CancellationToken cancellationToken)
        => _db.ContentItem.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Category == category && i.Id == id, cancellationToken);

    public async Task<int> AddAsync(ContentItemEntity item, CancellationToken cancellationToken)
    {
        await _db.ContentItem.AddAsync(item, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return item.Id;
    }

    public async Task<bool> UpdateAsync(ContentItemEntity item, CancellationToken cancellationToken)
    {
        var current = await _db.ContentItem
            .FirstOrDefaultAsync(i => i.Category == item.Category && i.Id == item.Id, cancellationToken);
        if (current is null) return false;

        current.Title = item.Title;
        current.Body = item.Body;
        current.ImageUrl = item.ImageUrl;
        current.DisplayOrder = item.DisplayOrder;
        current.IsActive = item.IsActive;
        current.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetActiveAsync(string category, int id, bool isActive, CancellationToken cancellationToken)
    {
        var current = await _db.ContentItem
            .FirstOrDefaultAsync(i => i.Category == category && i.Id == id, cancellationToken);
        if (current is null) return false;

        current.IsActive = isActive;
        current.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
