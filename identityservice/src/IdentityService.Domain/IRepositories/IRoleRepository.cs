using IdentityService.Domian.DTO;
using System.Threading;
 
namespace IdentityService.Domain.IRepositories
{
    public interface IRoleRepository : IBaseRepository
    {
        
        Task<Result> AddAsync(RoleModel role, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(RoleModel role, CancellationToken cancellationToken);
        Task<RoleModel> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<RoleModel>> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<RoleModel>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> AddRoleMenuAsync(int roleId, int menuId, CancellationToken cancellationToken);
        Task<bool> RemoveRoleMenuAsync(int roleId, int menuId, CancellationToken cancellationToken);
    }
}
