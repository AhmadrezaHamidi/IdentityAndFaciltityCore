using System.Threading;

namespace IdentityService.Domain.IRepositories
{
    public interface IPanelMenuRepository : IBaseRepository
    {
        Task<bool> AddAsync(PanelMenuModel menu, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(PanelMenuModel menu, CancellationToken cancellationToken);
        Task<PanelMenuModel> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<DataList<IEnumerable<PanelMenuModel>>> GetAllAsync(CancellationToken cancellationToken);
        Task<DataList<IEnumerable<PanelMenuModel>>> ReadAsync(int offset, int count, string term, CancellationToken cancellationToken);
        Task<IEnumerable<PanelMenuModel>> GetPanelRoleMenuAsync(string userName, CancellationToken cancellationToken);
        Task<IEnumerable<PanelMenuModel>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken);
        Task<List<PanelMenuRoleModel>> InquiryUserPanelMenu(long RoleId, CancellationToken cancellationToken);
        Task<IEnumerable<PanelMenuModel>> GetPanelRoleMenuAsyncv2(string userName, CancellationToken cancellationToken);
    }
}
