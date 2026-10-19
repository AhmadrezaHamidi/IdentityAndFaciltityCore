using IdentityService.Domain.Entity;
using IdentityService.Domain.Services.UserManagementPanel;
using OmidBank.Common.JwtHelper;
using static OmidBank.Common.Enums;

namespace IdentityService.Application.Handler.Admin.UserManagementPanel;

// ══════════════════════════════════════════════════════════════════════════
// تب ۲ «مدیریت مجوزها» (نقش) و تب ۳ «مدیریت گروه‌ها».
//
// هر دو تب در طرح Figma یک فرم دارند: نام، افزودن کاربر/کاربران با username،
// دسترسی/دسترسی‌ها، توضیحات، وضعیت فعال/غیرفعال — و هر دو با «بررسی» تکراری‌بودن
// نام و پیام موفقیت تمام می‌شوند. پس یک مجموعه هندلر با پارامتر Kind هر دو را
// پوشش می‌دهد: 0 = نقش، 1 = گروه.
// ══════════════════════════════════════════════════════════════════════════

public static class PanelRoleKind
{
    public const int Role = 0;
    public const int Group = 1;
}

public sealed record RoleListItem(
    int Id,
    string Title,
    string PersianTitle,
    string? Description,
    bool IsActive,
    DateTime? CreatedAtUtc,
    int? CreatedByPanelUserId);

public sealed record RoleDetail(
    int Id,
    string Title,
    string PersianTitle,
    string? Description,
    bool IsActive,
    DateTime? CreatedAtUtc,
    int? CreatedByPanelUserId,
    List<int> AccessIds,
    List<string> UserNames);

// ── گرید (ردیف، نام، تاریخ ایجاد، کاربر ایجادکننده، عملیات) ─────────────────
public sealed class SearchRolesQueryHandler
    : IBaseHandler<SearchRolesQuery, BaseMessageResponse<PagedResult<RoleListItem>>>
{
    private readonly IPanelAccessControlRepository _repository;

    public SearchRolesQueryHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<PagedResult<RoleListItem>>> Handle(
        SearchRolesQuery query, CancellationToken cancellationToken)
    {
        var (items, total) = await _repository.SearchRolesAsync(query.Kind, query.Term, query.Page, query.PageSize, cancellationToken);

        var response = MessageResponse.GenerateMessageResponse<PagedResult<RoleListItem>>(EResponseStatusCode.DoneSuccessfully);
        response.Data = new PagedResult<RoleListItem>(
            items.Select(r => new RoleListItem(r.Id, r.Title, r.PersianTitle, r.Description, r.IsActive, r.CreatedAt, r.CreatedByPanelUserId)).ToList(),
            total, query.Page <= 0 ? 1 : query.Page, query.PageSize <= 0 ? 20 : query.PageSize);
        return response;
    }
}

public sealed record SearchRolesQuery(int Kind, string? Term, int Page = 1, int PageSize = 20)
    : BaseQuery<BaseMessageResponse<PagedResult<RoleListItem>>>;

// ── جزئیات (پاپ‌آپ: کاربران، دسترسی‌ها، توضیحات، وضعیت) ─────────────────────
public sealed class GetRoleQueryHandler : IBaseHandler<GetRoleQuery, BaseMessageResponse<RoleDetail>>
{
    private readonly IPanelAccessControlRepository _repository;

    public GetRoleQueryHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<RoleDetail>> Handle(GetRoleQuery query, CancellationToken cancellationToken)
    {
        var role = await _repository.GetRoleAsync(query.Id, query.Kind, cancellationToken);
        if (role is null)
            return MessageResponse.GenerateMessageResponse<RoleDetail>(EResponseStatusCode.NotFound);

        var accessIds = await _repository.GetRoleMenuIdsAsync(role.Id, cancellationToken);
        var users = await _repository.GetRoleUsersAsync(role.Id, cancellationToken);

        var response = MessageResponse.GenerateMessageResponse<RoleDetail>(EResponseStatusCode.DoneSuccessfully);
        response.Data = new RoleDetail(
            role.Id, role.Title, role.PersianTitle, role.Description, role.IsActive,
            role.CreatedAt, role.CreatedByPanelUserId,
            accessIds, users.Select(u => u.UserName).ToList());
        return response;
    }
}

public sealed record GetRoleQuery(int Id, int Kind) : BaseQuery<BaseMessageResponse<RoleDetail>>;

// ── ایجاد ───────────────────────────────────────────────────────────────────
public sealed class CreateRoleCommandHandler : IBaseHandler<CreateRoleCommand, BaseMessageResponse<int>>
{
    private readonly IPanelAccessControlRepository _repository;
    private readonly IJwtService _jwtService;

    public CreateRoleCommandHandler(IPanelAccessControlRepository repository, IJwtService jwtService)
    {
        _repository = repository;
        _jwtService = jwtService;
    }

    public async Task<BaseMessageResponse<int>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.PersianTitle))
            return MessageResponse.GenerateMessageResponse<int>(EResponseStatusCode.WrongParameters);

        // طرح: «نام نقش تکراری است، قبلا با این نام نقشی ایجاد شده است.»
        // (و برای تب گروه‌ها همان پیام با واژهٔ گروه) — یکتایی به‌ازای هر Kind جداست.
        if (await _repository.RoleTitleExistsAsync(command.Kind, command.PersianTitle, exceptId: null, cancellationToken))
            return MessageResponse.GenerateMessageResponse<int>(EResponseStatusCode.DuplicateKey);

        var userIds = await ResolveUserIdsAsync(command.UserNames, cancellationToken);

        var role = new RoleEntity
        {
            Kind = command.Kind,
            PersianTitle = command.PersianTitle,
            Title = string.IsNullOrWhiteSpace(command.Title) ? command.PersianTitle : command.Title,
            Description = command.Description,
            IsActive = command.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedByPanelUserId = ResolvePanelUserId(command.Token),
        };

        var response = MessageResponse.GenerateMessageResponse<int>(EResponseStatusCode.DoneSuccessfully);
        response.Data = await _repository.AddRoleAsync(role, command.AccessIds ?? [], userIds, cancellationToken);
        return response;
    }

    private async Task<List<int>> ResolveUserIdsAsync(List<string>? userNames, CancellationToken cancellationToken)
    {
        if (userNames is null || userNames.Count == 0) return [];
        var users = await _repository.GetPanelUsersByUserNamesAsync(userNames, cancellationToken);
        return users.Select(u => u.Id).ToList();
    }

    private int? ResolvePanelUserId(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        var valid = _jwtService.ValidateToken(token);
        return valid.Item1 ? (int)valid.Item3.UserId : null;
    }
}

public sealed record CreateRoleCommand(
    int Kind,
    string PersianTitle,
    string? Title,
    string? Description,
    bool IsActive,
    List<int>? AccessIds,
    List<string>? UserNames,
    string? Token) : BaseCommand<BaseMessageResponse<int>>;

// ── ویرایش ──────────────────────────────────────────────────────────────────
public sealed class UpdateRoleCommandHandler : IBaseHandler<UpdateRoleCommand, BaseMessageResponse<bool>>
{
    private readonly IPanelAccessControlRepository _repository;

    public UpdateRoleCommandHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<bool>> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.PersianTitle))
            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.WrongParameters);

        if (await _repository.RoleTitleExistsAsync(command.Kind, command.PersianTitle, exceptId: command.Id, cancellationToken))
            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DuplicateKey);

        var userIds = new List<int>();
        if (command.UserNames is { Count: > 0 })
        {
            var users = await _repository.GetPanelUsersByUserNamesAsync(command.UserNames, cancellationToken);
            userIds = users.Select(u => u.Id).ToList();
        }

        var updated = await _repository.UpdateRoleAsync(new RoleEntity
        {
            Id = command.Id,
            Kind = command.Kind,
            PersianTitle = command.PersianTitle,
            Title = string.IsNullOrWhiteSpace(command.Title) ? command.PersianTitle : command.Title,
            Description = command.Description,
            IsActive = command.IsActive,
        }, command.AccessIds ?? [], userIds, cancellationToken);

        if (updated)
        {
            // مجوزهای یک نقش عوض شد؛ bump نسخه در رپازیتوری انجام شده و این‌جا فقط ثبت می‌شود.
            await _repository.WriteAuditAsync(new IdentityService.Domain.Entity.PanelSecurityAuditLogEntity
            {
                Action = "role.update",
                TargetType = command.Kind == 0 ? "Role" : "Group",
                TargetId = command.Id.ToString(),
                AfterJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    command.PersianTitle, command.IsActive, command.AccessIds, command.UserNames,
                }),
            }, cancellationToken);
        }

        if (!updated)
            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.NotFound);

        var response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully);
        response.Data = true;
        return response;
    }
}

public sealed record UpdateRoleCommand(
    int Id,
    int Kind,
    string PersianTitle,
    string? Title,
    string? Description,
    bool IsActive,
    List<int>? AccessIds,
    List<string>? UserNames) : BaseCommand<BaseMessageResponse<bool>>;
