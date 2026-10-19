//using IdentityService.Domain.Entity;
//using IdentityService.Domain.Services.CustomerManagement;
//using IdentityService.Infrastructure.Contexts;
//using Microsoft.EntityFrameworkCore;

//namespace IdentityService.Infrastructure.Repositories.CustomerManagement;

//public sealed class CustomerManagementRepository : ICustomerManagementRepository
//{
//    private readonly ApplicationDBContext _db;

//    public CustomerManagementRepository(ApplicationDBContext db)
//    {
//        _db = db;
//    }

//    public async Task<(List<UserEntity> Items, int Total)> SearchCustomersAsync(
//        string? nationalCode, int page, int pageSize, CancellationToken cancellationToken)
//    {
//        var query = _db.User.AsNoTracking().Include(u => u.Person).AsQueryable();

//        if (!string.IsNullOrWhiteSpace(nationalCode))
//            query = query.Where(u => u.Person.NationalCode == nationalCode);

//        var total = await query.CountAsync(cancellationToken);

//        var normalizedPage = page <= 0 ? 1 : page;
//        var normalizedPageSize = pageSize is <= 0 or > 200 ? 20 : pageSize;

//        var items = await query
//            .OrderByDescending(u => u.Id)
//            .Skip((normalizedPage - 1) * normalizedPageSize)
//            .Take(normalizedPageSize)
//            .ToListAsync(cancellationToken);

//        return (items, total);
//    }

//    public Task<UserEntity?> GetCustomerAsync(int userId, CancellationToken cancellationToken)
//        => _db.User.AsNoTracking().Include(u => u.Person)
//            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

//    public async Task<bool> SetBlockStatusAsync(int userId, bool blocked, CancellationToken cancellationToken)
//    {
//        var user = await _db.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
//        if (user is null) return false;

//        user.BlockByAdmin = blocked;
//        await _db.SaveChangesAsync(cancellationToken);
//        return true;
//    }

//    public Task<List<UserServiceAccessEntity>> GetServiceAccessAsync(int userId, CancellationToken cancellationToken)
//        => _db.UserServiceAccess.AsNoTracking()
//            .Where(a => a.UserId == userId)
//            .ToListAsync(cancellationToken);

//    public async Task SetServiceAccessAsync(
//        int userId, string serviceKey, bool isEnabled, int? updatedByPanelUserId, CancellationToken cancellationToken)
//    {
//        var existing = await _db.UserServiceAccess
//            .FirstOrDefaultAsync(a => a.UserId == userId && a.ServiceKey == serviceKey, cancellationToken);

//        if (existing is null)
//        {
//            await _db.UserServiceAccess.AddAsync(new UserServiceAccessEntity
//            {
//                UserId = userId,
//                ServiceKey = serviceKey,
//                IsEnabled = isEnabled,
//                UpdatedAtUtc = DateTime.UtcNow,
//                UpdatedByPanelUserId = updatedByPanelUserId,
//            }, cancellationToken);
//        }
//        else
//        {
//            existing.IsEnabled = isEnabled;
//            existing.UpdatedAtUtc = DateTime.UtcNow;
//            existing.UpdatedByPanelUserId = updatedByPanelUserId;
//        }

//        await _db.SaveChangesAsync(cancellationToken);
//    }
//}
