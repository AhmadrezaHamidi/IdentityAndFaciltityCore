using IdentityService.Domain.Services.UserManagementPanel;
using static OmidBank.Common.Enums;

namespace IdentityService.Application.Handler.Admin.UserManagementPanel;

// ══════════════════════════════════════════════════════════════════════════
// «هر Role/Group به چه دسترسی‌هایی دسترسی دارد» — ماتریس مجوز/گروه × دسترسی.
// بدون هیچ جدول یا کوئری جدید: فقط ترکیب متدهای موجود IPanelAccessControlRepository
// (SearchRolesAsync، SearchAccessesAsync، GetRoleMenuIdsAsync) که تب‌های ۱ و ۲/۳
// «مدیریت کاربران» از قبل از آن‌ها استفاده می‌کنند.
// ══════════════════════════════════════════════════════════════════════════

public sealed record RoleAccessMatrixRowResponse(
    int RoleId, string Title, string PersianTitle, int Kind, List<int> MenuIds);

public sealed record RoleAccessMatrixResponse(
    List<MenuSummaryResponse> Menus, List<RoleAccessMatrixRowResponse> Roles);

public sealed record MenuSummaryResponse(int Id, string Title);

public sealed class GetRoleAccessMatrixQueryHandler
    : IBaseHandler<GetRoleAccessMatrixQuery, BaseMessageResponse<RoleAccessMatrixResponse>>
{
    private const int MaxItems = 5000;

    private readonly IPanelAccessControlRepository _repository;

    public GetRoleAccessMatrixQueryHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<RoleAccessMatrixResponse>> Handle(
        GetRoleAccessMatrixQuery query, CancellationToken cancellationToken)
    {
        var (menus, _) = await _repository.SearchAccessesAsync(null, 1, MaxItems, cancellationToken);

        var (roles, _) = await _repository.SearchRolesAsync(PanelRoleKind.Role, null, 1, MaxItems, cancellationToken);
        var (groups, _) = await _repository.SearchRolesAsync(PanelRoleKind.Group, null, 1, MaxItems, cancellationToken);
        var allRoles = roles.Concat(groups).ToList();

        var rows = new List<RoleAccessMatrixRowResponse>();
        foreach (var role in allRoles)
        {
            var menuIds = await _repository.GetRoleMenuIdsAsync(role.Id, cancellationToken);
            rows.Add(new RoleAccessMatrixRowResponse(role.Id, role.Title, role.PersianTitle, role.Kind, menuIds));
        }

        var response = MessageResponse.GenerateMessageResponse<RoleAccessMatrixResponse>(EResponseStatusCode.DoneSuccessfully);
        response.Data = new RoleAccessMatrixResponse(
            menus.Select(m => new MenuSummaryResponse(m.Id, m.Title)).ToList(),
            rows);
        return response;
    }
}

public sealed record GetRoleAccessMatrixQuery : BaseQuery<BaseMessageResponse<RoleAccessMatrixResponse>>;
