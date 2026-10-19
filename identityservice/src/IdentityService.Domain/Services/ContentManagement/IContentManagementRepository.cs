using IdentityService.Domain.Entity.ContentManagement;

namespace IdentityService.Domain.Services.ContentManagement;

/// <summary>
/// CRUD عمومی برای هر چهار زیرمنوی «مدیریت محتوا» — یک پیاده‌سازی، چهار دسته.
/// مستقیم با EF.
/// </summary>
public interface IContentManagementRepository
{
    Task<List<ContentItemEntity>> ListAsync(string category, CancellationToken cancellationToken);

    Task<ContentItemEntity?> GetAsync(string category, int id, CancellationToken cancellationToken);

    Task<int> AddAsync(ContentItemEntity item, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(ContentItemEntity item, CancellationToken cancellationToken);

    Task<bool> SetActiveAsync(string category, int id, bool isActive, CancellationToken cancellationToken);
}
