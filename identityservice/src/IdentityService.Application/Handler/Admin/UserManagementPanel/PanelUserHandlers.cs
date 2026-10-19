using IdentityService.Domain.Services.UserManagementPanel;
using static OmidBank.Common.Enums;

namespace IdentityService.Application.Handler.Admin.UserManagementPanel;

// ══════════════════════════════════════════════════════════════════════════
// تب ۴ — «مدیریت کاربران» پنل ادمین.
//
// ساخت کاربر پنل عمداً این‌جا نیست: در نسخهٔ فعلی ساخت کاربر با رمز عبور از
// مسیر موجود Membership انجام می‌شود و تکرارش این‌جا یعنی دو جای متفاوت برای
// هش‌کردن رمز. این تب فهرست/جزئیات و فعال-غیرفعال‌سازی را پوشش می‌دهد؛
// نقش‌دهی به کاربر هم از تب مجوزها/گروه‌ها انجام می‌شود (همان جدول واسط).
// ══════════════════════════════════════════════════════════════════════════

public sealed record PanelUserListItem(
    int Id,
    string UserName,
    string? Email,
    int PersonId,
    bool IsActive,
    DateTime CreationTime);

// ── گرید ────────────────────────────────────────────────────────────────────
public sealed class SearchPanelUsersQueryHandler
    : IBaseHandler<SearchPanelUsersQuery, BaseMessageResponse<PagedResult<PanelUserListItem>>>
{
    private readonly IPanelAccessControlRepository _repository;

    public SearchPanelUsersQueryHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<PagedResult<PanelUserListItem>>> Handle(
        SearchPanelUsersQuery query, CancellationToken cancellationToken)
    {
        var (items, total) = await _repository.SearchPanelUsersAsync(query.Term, query.Page, query.PageSize, cancellationToken);

        var response = MessageResponse.GenerateMessageResponse<PagedResult<PanelUserListItem>>(EResponseStatusCode.DoneSuccessfully);
        response.Data = new PagedResult<PanelUserListItem>(
            items.Select(u => new PanelUserListItem(u.Id, u.UserName, u.Email, u.PersonId, u.IsActive, u.CreationTime)).ToList(),
            total, query.Page <= 0 ? 1 : query.Page, query.PageSize <= 0 ? 20 : query.PageSize);
        return response;
    }
}

public sealed record SearchPanelUsersQuery(string? Term, int Page = 1, int PageSize = 20)
    : BaseQuery<BaseMessageResponse<PagedResult<PanelUserListItem>>>;

// ── جزئیات ──────────────────────────────────────────────────────────────────
public sealed class GetPanelUserQueryHandler : IBaseHandler<GetPanelUserQuery, BaseMessageResponse<PanelUserListItem>>
{
    private readonly IPanelAccessControlRepository _repository;

    public GetPanelUserQueryHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<PanelUserListItem>> Handle(GetPanelUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _repository.GetPanelUserAsync(query.Id, cancellationToken);
        if (user is null)
            return MessageResponse.GenerateMessageResponse<PanelUserListItem>(EResponseStatusCode.NotFound);

        var response = MessageResponse.GenerateMessageResponse<PanelUserListItem>(EResponseStatusCode.DoneSuccessfully);
        response.Data = new PanelUserListItem(user.Id, user.UserName, user.Email, user.PersonId, user.IsActive, user.CreationTime);
        return response;
    }
}

public sealed record GetPanelUserQuery(int Id) : BaseQuery<BaseMessageResponse<PanelUserListItem>>;

// ── فعال / غیرفعال ─────────────────────────────────────────────────────────
public sealed class SetPanelUserActiveCommandHandler : IBaseHandler<SetPanelUserActiveCommand, BaseMessageResponse<bool>>
{
    private readonly IPanelAccessControlRepository _repository;

    public SetPanelUserActiveCommandHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<bool>> Handle(SetPanelUserActiveCommand command, CancellationToken cancellationToken)
    {
        var before = await _repository.GetPanelUserAsync(command.Id, cancellationToken);

        var updated = await _repository.SetPanelUserActiveAsync(command.Id, command.IsActive, cancellationToken);
        if (!updated)
            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.NotFound);

        await _repository.WriteAuditAsync(new IdentityService.Domain.Entity.PanelSecurityAuditLogEntity
        {
            Action = "panel-user.active.change",
            ActorPanelUserId = command.ActorPanelUserId,
            TargetType = "PanelUser",
            TargetId = command.Id.ToString(),
            BeforeJson = System.Text.Json.JsonSerializer.Serialize(new { IsActive = before?.IsActive }),
            AfterJson = System.Text.Json.JsonSerializer.Serialize(new { command.IsActive }),
        }, cancellationToken);

        var response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully);
        response.Data = true;
        return response;
    }
}

public sealed record SetPanelUserActiveCommand(int Id, bool IsActive, int? ActorPanelUserId = null)
    : BaseCommand<BaseMessageResponse<bool>>;
