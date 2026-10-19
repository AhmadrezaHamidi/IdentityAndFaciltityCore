using Admin.Api.Filters;
using IdentityService.Domain.Authorization;
using IdentityService.Application.Handler.Admin.UserManagementPanel;
using OmidBank.Common.Attribute;

namespace Admin.Api.Controllers.V2.UserManagement
{
    /// <summary>
    /// ماژول «مدیریت کاربران» پنل ادمین — طبق فلوچارت Figma «Amin panel(2)».
    ///
    /// چهار تب طرح:
    ///   ۱. مدیریت دسترسی‌ها  → Accesses/*
    ///   ۲. مدیریت مجوزها     → Roles/*      (kind = 0)
    ///   ۳. مدیریت گروه‌ها     → Groups/*     (kind = 1، همان جدول با Kind متفاوت)
    ///   ۴. مدیریت کاربران    → PanelUsers/*
    ///
    /// نمایش خود منو در sidebar بر عهدهٔ MainMenu است؛ اگر کاربر مجوز این منو را
    /// نداشته باشد منو نمایش داده نمی‌شود و این مسیرها هم برایش صدا زده نمی‌شوند.
    /// </summary>
    [Route("UserManagement")]
    [ApiVersion("2")]
    public class UserManagementController : BaseIdentityApplicationApi
    {
        // ── تب ۱: مدیریت دسترسی‌ها ────────────────────────────────────────────────
        [HttpGet("Accesses")]
        [RequirePanelPermission(AdminPermissionKeys.PermissionsRead)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> SearchAccesses([FromQuery] string? term, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new SearchAccessesQuery(term, page, pageSize), cancellationToken));

        [HttpGet("Accesses/{id:int}")]
        [RequirePanelPermission(AdminPermissionKeys.PermissionsRead)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> GetAccess(int id, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new GetAccessQuery(id), cancellationToken));

        [HttpPost("Accesses")]
        [RequirePanelPermission(AdminPermissionKeys.PermissionsWrite)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> CreateAccess([FromBody] CreateAccessCommand command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(command, cancellationToken));

        [HttpPut("Accesses/{id:int}")]
        [RequirePanelPermission(AdminPermissionKeys.PermissionsWrite)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> UpdateAccess(int id, [FromBody] UpdateAccessCommand command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(command with { Id = id }, cancellationToken));

        // ── تب ۲: مدیریت مجوزها (نقش) ────────────────────────────────────────────
        [HttpGet("Roles")]
        [RequirePanelPermission(AdminPermissionKeys.RolesRead)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> SearchRoles([FromQuery] string? term, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new SearchRolesQuery(PanelRoleKind.Role, term, page, pageSize), cancellationToken));

        [HttpGet("Roles/{id:int}")]
        [RequirePanelPermission(AdminPermissionKeys.RolesRead)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> GetRole(int id, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new GetRoleQuery(id, PanelRoleKind.Role), cancellationToken));

        [HttpPost("Roles")]
        [RequirePanelPermission(AdminPermissionKeys.RolesWrite)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(
                command with { Kind = PanelRoleKind.Role, Token = _baseRequest.RequestInfo.Token }, cancellationToken));

        [HttpPut("Roles/{id:int}")]
        [RequirePanelPermission(AdminPermissionKeys.RolesWrite)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleCommand command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(command with { Id = id, Kind = PanelRoleKind.Role }, cancellationToken));

        // ── تب ۳: مدیریت گروه‌ها ─────────────────────────────────────────────────
        [HttpGet("Groups")]
        [RequirePanelPermission(AdminPermissionKeys.GroupsRead)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> SearchGroups([FromQuery] string? term, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new SearchRolesQuery(PanelRoleKind.Group, term, page, pageSize), cancellationToken));

        [HttpGet("Groups/{id:int}")]
        [RequirePanelPermission(AdminPermissionKeys.GroupsRead)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> GetGroup(int id, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new GetRoleQuery(id, PanelRoleKind.Group), cancellationToken));

        [HttpPost("Groups")]
        [RequirePanelPermission(AdminPermissionKeys.GroupsWrite)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> CreateGroup([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(
                command with { Kind = PanelRoleKind.Group, Token = _baseRequest.RequestInfo.Token }, cancellationToken));

        [HttpPut("Groups/{id:int}")]
        [RequirePanelPermission(AdminPermissionKeys.GroupsWrite)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] UpdateRoleCommand command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(command with { Id = id, Kind = PanelRoleKind.Group }, cancellationToken));

        // ── تب ۴: مدیریت کاربران پنل ─────────────────────────────────────────────
        [HttpGet("PanelUsers")]
        [RequirePanelPermission(AdminPermissionKeys.UsersRead)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> SearchPanelUsers([FromQuery] string? term, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new SearchPanelUsersQuery(term, page, pageSize), cancellationToken));

        [HttpGet("PanelUsers/{id:int}")]
        [RequirePanelPermission(AdminPermissionKeys.UsersRead)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> GetPanelUser(int id, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new GetPanelUserQuery(id), cancellationToken));

        [HttpPut("PanelUsers/{id:int}/Active")]
        [RequirePanelPermission(AdminPermissionKeys.UsersWrite)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> SetPanelUserActive(int id, [FromQuery] bool isActive, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new SetPanelUserActiveCommand(id, isActive, CurrentPanelUserId), cancellationToken));

        // ── ماتریس دسترسی: هر Role/Group به چه دسترسی‌هایی دسترسی دارد ──────────────
        // POST /UserManagement/PanelUsers — فرم سند: نام کاربری، رمز، نام کامل، کد کاربر،
        // نام و کد شعبه، و انتساب نقش با دامنه.
        [HttpPost("PanelUsers")]
        [RequirePanelPermission(AdminPermissionKeys.UsersWrite)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> CreatePanelUser([FromBody] CreatePanelUserCommand command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(command with { ActorPanelUserId = CurrentPanelUserId }, cancellationToken));

        [HttpPut("PanelUsers/{id:int}")]
        [RequirePanelPermission(AdminPermissionKeys.UsersWrite)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> UpdatePanelUser(int id, [FromBody] UpdatePanelUserCommand command, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(command with { Id = id, ActorPanelUserId = CurrentPanelUserId }, cancellationToken));

        // انتساب نقش جدا از ویرایش کاربر، تا فرم بتواند فقط نقش‌ها را بفرستد.
        [HttpPut("PanelUsers/{id:int}/Roles")]
        [RequirePanelPermission(AdminPermissionKeys.UsersAssign)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> AssignRoles(int id, [FromBody] List<RoleAssignmentDto> roles, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new UpdatePanelUserRolesCommand(id, roles, CurrentPanelUserId), cancellationToken));

        // ── لاگ امنیتی — دستهٔ «کاربران» در گزارش تغییرات ─────────────────────────
        [HttpGet("SecurityAudit")]
        [RequirePanelPermission(AdminPermissionKeys.AuditRead)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> SearchSecurityAudit([FromQuery] string? action, [FromQuery] string? targetType, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new SearchSecurityAuditQuery(action, targetType, page, pageSize), cancellationToken));

        [HttpGet("AccessMatrix")]
        [RequirePanelPermission(AdminPermissionKeys.UsersRead)]
        [AuthenticationUserAdminByCookie]
        public async Task<IActionResult> GetAccessMatrix(CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(new GetRoleAccessMatrixQuery(), cancellationToken));

        /// <summary>شناسهٔ کاربر جاری از نشستی که PanelPermissionFilter گذاشته — برای لاگ امنیتی.</summary>
        private int? CurrentPanelUserId =>
            HttpContext.Items.TryGetValue(PanelPermissionFilter.CurrentUserKey, out var value)
            && value is IdentityService.Domain.Authorization.PanelSession session
                ? session.PanelUserId
                : null;
    }

}
