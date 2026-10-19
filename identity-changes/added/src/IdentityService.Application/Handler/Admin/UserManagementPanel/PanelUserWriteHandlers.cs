using IdentityService.Domain.Authorization;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Models.User;
using IdentityService.Domain.Services.UserManagementPanel;
using static OmidBank.Common.Enums;

namespace IdentityService.Application.Handler.Admin.UserManagementPanel;

// ══════════════════════════════════════════════════════════════════════════
// ساخت و ویرایش کاربر پنل — فرم سند:
//   نام کاربری، رمز، نام کامل، کد کاربر، نام و کد شعبه، و انتساب نقش با دامنه.
//
// رمز با UserModel.SetPassword هش می‌شود، همان متدی که مسیر Membership استفاده
// می‌کند. هیچ هش دومی این‌جا نوشته نشده: دو جای متفاوت برای هش‌کردن رمز یعنی
// روزی یکی‌شان عوض می‌شود و ورود می‌شکند.
// ══════════════════════════════════════════════════════════════════════════

public sealed record RoleAssignmentDto(int RoleId, AccessScopeType ScopeType, string? ScopeId);

public sealed record PanelUserDetails(
    int Id,
    string UserName,
    string? Email,
    string? FullName,
    string? UserCode,
    string? BranchCode,
    string? BranchName,
    bool IsActive,
    int PermissionVersion,
    List<RoleAssignmentDto> Roles);

// ── ساخت ────────────────────────────────────────────────────────────────────
public sealed class CreatePanelUserCommandHandler
    : IBaseHandler<CreatePanelUserCommand, BaseMessageResponse<PanelUserDetails>>
{
    private readonly IPanelAccessControlRepository _repository;

    public CreatePanelUserCommandHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<PanelUserDetails>> Handle(
        CreatePanelUserCommand command, CancellationToken cancellationToken)
    {
        var invalid = await Validate(_repository, command.UserName, command.UserCode, command.Roles, null, cancellationToken);
        if (invalid is not null) return Fail(invalid);

        if (string.IsNullOrWhiteSpace(command.Password))
            return Fail("رمز عبور الزامی است.");

        if (command.PersonId <= 0)
            return Fail("شناسهٔ شخص الزامی است.");

        // هش از همان مسیر Membership
        var credentials = new UserModel();
        credentials.SetPassword(command.Password);

        var user = new PanelUserEntity
        {
            PersonId = command.PersonId,
            UserName = command.UserName.Trim(),
            PasswordHash = credentials.Password,
            Email = command.Email?.Trim()!,
            FullName = command.FullName?.Trim(),
            UserCode = command.UserCode?.Trim(),
            BranchCode = command.BranchCode?.Trim(),
            BranchName = command.BranchName?.Trim(),
            IsActive = true,
        };

        var id = await _repository.AddPanelUserAsync(user, ToAssignments(command.Roles, command.ActorPanelUserId), cancellationToken);

        await _repository.WriteAuditAsync(new PanelSecurityAuditLogEntity
        {
            Action = "panel-user.create",
            ActorPanelUserId = command.ActorPanelUserId,
            TargetType = "PanelUser",
            TargetId = id.ToString(),
            AfterJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                user.UserName, user.FullName, user.UserCode, user.BranchCode, command.Roles,
            }),
        }, cancellationToken);

        return await Read(_repository, id, cancellationToken);
    }

    private static BaseMessageResponse<PanelUserDetails> Fail(string message)
    {
        var response = MessageResponse.GenerateMessageResponse<PanelUserDetails>(EResponseStatusCode.BadRequest);
        response.Message = message;
        return response;
    }

    internal static List<RoleAssignment> ToAssignments(List<RoleAssignmentDto>? roles, int? actor) =>
        (roles ?? []).Select(r => new RoleAssignment(r.RoleId, r.ScopeType, r.ScopeId, actor)).ToList();

    /// <summary>
    /// یکتایی نام کاربری و کد کاربر، و اینکه دامنهٔ هر انتساب با AllowedScopeType نقش بخواند.
    /// بدون این چک، «کاربر شعبه» را می‌شد سراسری داد و معیار ۵ بی‌اثر می‌شد.
    /// </summary>
    internal static async Task<string?> Validate(
        IPanelAccessControlRepository repository,
        string? userName,
        string? userCode,
        List<RoleAssignmentDto>? roles,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return "نام کاربری الزامی است.";

        if (await repository.PanelUserNameExistsAsync(userName.Trim(), exceptId, cancellationToken))
            return "این نام کاربری قبلاً استفاده شده است.";

        if (!string.IsNullOrWhiteSpace(userCode) &&
            await repository.PanelUserCodeExistsAsync(userCode.Trim(), exceptId, cancellationToken))
            return "این کد کاربر قبلاً استفاده شده است.";

        foreach (var role in roles ?? [])
        {
            if (role.ScopeType != AccessScopeType.Global && string.IsNullOrWhiteSpace(role.ScopeId))
                return "برای دامنهٔ شعبه یا شرکت، شناسهٔ دامنه الزامی است.";

            if (role.ScopeType == AccessScopeType.Global && !string.IsNullOrWhiteSpace(role.ScopeId))
                return "دامنهٔ سراسری شناسه نمی‌گیرد.";
        }

        return null;
    }

    internal static async Task<BaseMessageResponse<PanelUserDetails>> Read(
        IPanelAccessControlRepository repository, int id, CancellationToken cancellationToken)
    {
        var saved = await repository.GetPanelUserWithPermissionsAsync(id, cancellationToken);
        if (saved is null)
            return MessageResponse.GenerateMessageResponse<PanelUserDetails>(EResponseStatusCode.NotFound);

        var response = MessageResponse.GenerateMessageResponse<PanelUserDetails>(EResponseStatusCode.DoneSuccessfully);
        response.Data = new PanelUserDetails(
            saved.Id, saved.UserName, saved.Email, saved.FullName, saved.UserCode,
            saved.BranchCode, saved.BranchName, saved.IsActive, saved.PermissionVersion,
            saved.PanelUserRoles.Select(a => new RoleAssignmentDto(a.RoleId, a.ScopeType, a.ScopeId)).ToList());
        return response;
    }
}

public sealed record CreatePanelUserCommand(
    int PersonId,
    string UserName,
    string Password,
    string? Email,
    string? FullName,
    string? UserCode,
    string? BranchCode,
    string? BranchName,
    List<RoleAssignmentDto>? Roles,
    int? ActorPanelUserId = null) : BaseCommand<BaseMessageResponse<PanelUserDetails>>;

// ── ویرایش ──────────────────────────────────────────────────────────────────
public sealed class UpdatePanelUserCommandHandler
    : IBaseHandler<UpdatePanelUserCommand, BaseMessageResponse<PanelUserDetails>>
{
    private readonly IPanelAccessControlRepository _repository;

    public UpdatePanelUserCommandHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<PanelUserDetails>> Handle(
        UpdatePanelUserCommand command, CancellationToken cancellationToken)
    {
        var before = await _repository.GetPanelUserWithPermissionsAsync(command.Id, cancellationToken);
        if (before is null)
            return MessageResponse.GenerateMessageResponse<PanelUserDetails>(EResponseStatusCode.NotFound);

        var invalid = await CreatePanelUserCommandHandler.Validate(
            _repository, command.UserName, command.UserCode, command.Roles, command.Id, cancellationToken);

        if (invalid is not null)
        {
            var bad = MessageResponse.GenerateMessageResponse<PanelUserDetails>(EResponseStatusCode.BadRequest);
            bad.Message = invalid;
            return bad;
        }

        string? passwordHash = null;
        if (!string.IsNullOrWhiteSpace(command.NewPassword))
        {
            var credentials = new UserModel();
            credentials.SetPassword(command.NewPassword);
            passwordHash = credentials.Password;
        }

        var updated = await _repository.UpdatePanelUserAsync(new PanelUserEntity
        {
            Id = command.Id,
            UserName = command.UserName.Trim(),
            PasswordHash = passwordHash!,
            Email = command.Email?.Trim()!,
            FullName = command.FullName?.Trim(),
            UserCode = command.UserCode?.Trim(),
            BranchCode = command.BranchCode?.Trim(),
            BranchName = command.BranchName?.Trim(),
            IsActive = command.IsActive,
        },
        // null یعنی فرم نقش‌ها را نفرستاده، پس دست نخورند.
        command.Roles is null ? null : CreatePanelUserCommandHandler.ToAssignments(command.Roles, command.ActorPanelUserId),
        cancellationToken);

        if (!updated)
            return MessageResponse.GenerateMessageResponse<PanelUserDetails>(EResponseStatusCode.NotFound);

        await _repository.WriteAuditAsync(new PanelSecurityAuditLogEntity
        {
            Action = "panel-user.update",
            ActorPanelUserId = command.ActorPanelUserId,
            TargetType = "PanelUser",
            TargetId = command.Id.ToString(),
            BeforeJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                before.UserName, before.FullName, before.UserCode, before.BranchCode, before.IsActive,
                Roles = before.PanelUserRoles.Select(a => new { a.RoleId, a.ScopeType, a.ScopeId }),
            }),
            AfterJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                command.UserName, command.FullName, command.UserCode, command.BranchCode, command.IsActive,
                Roles = command.Roles,
            }),
        }, cancellationToken);

        return await CreatePanelUserCommandHandler.Read(_repository, command.Id, cancellationToken);
    }
}

public sealed record UpdatePanelUserCommand(
    int Id,
    string UserName,
    string? NewPassword,
    string? Email,
    string? FullName,
    string? UserCode,
    string? BranchCode,
    string? BranchName,
    bool IsActive,
    List<RoleAssignmentDto>? Roles,
    int? ActorPanelUserId = null) : BaseCommand<BaseMessageResponse<PanelUserDetails>>;

// ── لاگ امنیتی ──────────────────────────────────────────────────────────────
public sealed class SearchSecurityAuditQueryHandler
    : IBaseHandler<SearchSecurityAuditQuery, BaseMessageResponse<PagedResult<SecurityAuditItem>>>
{
    private readonly IPanelAccessControlRepository _repository;

    public SearchSecurityAuditQueryHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<PagedResult<SecurityAuditItem>>> Handle(
        SearchSecurityAuditQuery query, CancellationToken cancellationToken)
    {
        var (items, total) = await _repository.SearchAuditAsync(
            query.Action, query.TargetType, query.Page, query.PageSize, cancellationToken);

        var response = MessageResponse.GenerateMessageResponse<PagedResult<SecurityAuditItem>>(EResponseStatusCode.DoneSuccessfully);
        response.Data = new PagedResult<SecurityAuditItem>(
            items.Select(x => new SecurityAuditItem(
                x.Id, x.Action, x.ActorPanelUserId, x.TargetType, x.TargetId,
                x.BeforeJson, x.AfterJson, x.CreatedAt)).ToList(),
            total, query.Page <= 0 ? 1 : query.Page, query.PageSize <= 0 ? 20 : query.PageSize);
        return response;
    }
}

public sealed record SecurityAuditItem(
    long Id,
    string Action,
    int? ActorPanelUserId,
    string TargetType,
    string? TargetId,
    string? BeforeJson,
    string? AfterJson,
    DateTime CreatedAt);

public sealed record SearchSecurityAuditQuery(string? Action, string? TargetType, int Page = 1, int PageSize = 20)
    : BaseQuery<BaseMessageResponse<PagedResult<SecurityAuditItem>>>;

// ── انتساب نقش، جدا از ویرایش کاربر ─────────────────────────────────────────
public sealed class UpdatePanelUserRolesCommandHandler
    : IBaseHandler<UpdatePanelUserRolesCommand, BaseMessageResponse<PanelUserDetails>>
{
    private readonly IPanelAccessControlRepository _repository;

    public UpdatePanelUserRolesCommandHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<PanelUserDetails>> Handle(
        UpdatePanelUserRolesCommand command, CancellationToken cancellationToken)
    {
        var before = await _repository.GetPanelUserWithPermissionsAsync(command.Id, cancellationToken);
        if (before is null)
            return MessageResponse.GenerateMessageResponse<PanelUserDetails>(EResponseStatusCode.NotFound);

        var invalid = await CreatePanelUserCommandHandler.Validate(
            _repository, before.UserName, before.UserCode, command.Roles, command.Id, cancellationToken);

        if (invalid is not null)
        {
            var bad = MessageResponse.GenerateMessageResponse<PanelUserDetails>(EResponseStatusCode.BadRequest);
            bad.Message = invalid;
            return bad;
        }

        var updated = await _repository.UpdatePanelUserAsync(new PanelUserEntity
        {
            Id = before.Id,
            UserName = before.UserName,
            Email = before.Email,
            FullName = before.FullName,
            UserCode = before.UserCode,
            BranchCode = before.BranchCode,
            BranchName = before.BranchName,
            IsActive = before.IsActive,
        },
        CreatePanelUserCommandHandler.ToAssignments(command.Roles, command.ActorPanelUserId),
        cancellationToken);

        if (!updated)
            return MessageResponse.GenerateMessageResponse<PanelUserDetails>(EResponseStatusCode.NotFound);

        await _repository.WriteAuditAsync(new PanelSecurityAuditLogEntity
        {
            Action = "panel-user.roles.update",
            ActorPanelUserId = command.ActorPanelUserId,
            TargetType = "PanelUser",
            TargetId = command.Id.ToString(),
            BeforeJson = System.Text.Json.JsonSerializer.Serialize(
                before.PanelUserRoles.Select(a => new { a.RoleId, a.ScopeType, a.ScopeId })),
            AfterJson = System.Text.Json.JsonSerializer.Serialize(command.Roles),
        }, cancellationToken);

        return await CreatePanelUserCommandHandler.Read(_repository, command.Id, cancellationToken);
    }
}

public sealed record UpdatePanelUserRolesCommand(
    int Id,
    List<RoleAssignmentDto> Roles,
    int? ActorPanelUserId = null) : BaseCommand<BaseMessageResponse<PanelUserDetails>>;
