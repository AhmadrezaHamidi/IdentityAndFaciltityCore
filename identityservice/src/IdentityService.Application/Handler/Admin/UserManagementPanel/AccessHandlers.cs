using IdentityService.Domain.Entity;
using IdentityService.Domain.Services.UserManagementPanel;
using static OmidBank.Common.Enums;

namespace IdentityService.Application.Handler.Admin.UserManagementPanel;

// ══════════════════════════════════════════════════════════════════════════
// تب ۱ — «مدیریت دسترسی‌ها» (فلوچارت Figma، ماژول مدیریت کاربران)
//
//   انتخاب تب → نمایش گرید (جستجو بر اساس نام + خروجی اکسل + دکمهٔ ایجاد)
//     ├─ ایجاد   → درج اطلاعات → پیام «آیا اطمینان دارید؟» → ثبت
//     ├─ ویرایش  → ویرایش اطلاعات → پیام تأیید → ثبت
//     └─ جزئیات  → پاپ‌آپ (توضیحات، وضعیت فعال/غیرفعال)
//
// «دسترسی» روی جدول PanelMenu می‌نشیند — همان واحد اتمی مجوز که RolePanelMenu
// به آن ارجاع می‌دهد. جدول جدیدی ساخته نشد؛ فقط ستون Description اضافه شد.
// ══════════════════════════════════════════════════════════════════════════

public sealed record AccessListItem(
    int Id,
    string Title,
    string? Description,
    int? ParentId,
    bool IsActive,
    DateTime CreationTime);

public sealed record PagedResult<T>(List<T> Items, int Total, int Page, int PageSize);

// ── گرید ────────────────────────────────────────────────────────────────────
public sealed class SearchAccessesQueryHandler
    : IBaseHandler<SearchAccessesQuery, BaseMessageResponse<PagedResult<AccessListItem>>>
{
    private readonly IPanelAccessControlRepository _repository;

    public SearchAccessesQueryHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<PagedResult<AccessListItem>>> Handle(
        SearchAccessesQuery query, CancellationToken cancellationToken)
    {
        var (items, total) = await _repository.SearchAccessesAsync(query.Term, query.Page, query.PageSize, cancellationToken);

        var response = MessageResponse.GenerateMessageResponse<PagedResult<AccessListItem>>(EResponseStatusCode.DoneSuccessfully);
        response.Data = new PagedResult<AccessListItem>(
            items.Select(m => new AccessListItem(m.Id, m.Title, m.Description, m.ParentId, m.IsActive, m.CreationTime)).ToList(),
            total, query.Page <= 0 ? 1 : query.Page, query.PageSize <= 0 ? 20 : query.PageSize);
        return response;
    }
}

public sealed record SearchAccessesQuery(string? Term, int Page = 1, int PageSize = 20)
    : BaseQuery<BaseMessageResponse<PagedResult<AccessListItem>>>;

// ── جزئیات ──────────────────────────────────────────────────────────────────
public sealed class GetAccessQueryHandler : IBaseHandler<GetAccessQuery, BaseMessageResponse<AccessListItem>>
{
    private readonly IPanelAccessControlRepository _repository;

    public GetAccessQueryHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<AccessListItem>> Handle(GetAccessQuery query, CancellationToken cancellationToken)
    {
        var access = await _repository.GetAccessAsync(query.Id, cancellationToken);
        if (access is null)
            return MessageResponse.GenerateMessageResponse<AccessListItem>(EResponseStatusCode.NotFound);

        var response = MessageResponse.GenerateMessageResponse<AccessListItem>(EResponseStatusCode.DoneSuccessfully);
        response.Data = new AccessListItem(access.Id, access.Title, access.Description, access.ParentId, access.IsActive, access.CreationTime);
        return response;
    }
}

public sealed record GetAccessQuery(int Id) : BaseQuery<BaseMessageResponse<AccessListItem>>;

// ── ایجاد ───────────────────────────────────────────────────────────────────
public sealed class CreateAccessCommandHandler : IBaseHandler<CreateAccessCommand, BaseMessageResponse<int>>
{
    private readonly IPanelAccessControlRepository _repository;

    public CreateAccessCommandHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<int>> Handle(CreateAccessCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Title))
            return MessageResponse.GenerateMessageResponse<int>(EResponseStatusCode.WrongParameters);

        // طرح: «نام تکراری است، قبلاً با این نام ایجاد شده است.»
        if (await _repository.AccessTitleExistsAsync(command.Title, exceptId: null, cancellationToken))
            return MessageResponse.GenerateMessageResponse<int>(EResponseStatusCode.DuplicateKey);

        var access = new PanelMenuEntity
        {
            Title = command.Title,
            EnTitle = command.EnTitle,
            Url = command.Url,
            IconName = command.IconName,
            ParentId = command.ParentId,
            Description = command.Description,
            IsActive = command.IsActive,
            CreationTime = DateTime.Now,
            IsRootMenu = command.ParentId is null,
            IsInternalMenu = false,
        };

        var response = MessageResponse.GenerateMessageResponse<int>(EResponseStatusCode.DoneSuccessfully);
        response.Data = await _repository.AddAccessAsync(access, cancellationToken);
        return response;
    }
}

public sealed record CreateAccessCommand(
    string Title,
    string? EnTitle,
    string? Url,
    string? IconName,
    int? ParentId,
    string? Description,
    bool IsActive) : BaseCommand<BaseMessageResponse<int>>;

// ── ویرایش ──────────────────────────────────────────────────────────────────
public sealed class UpdateAccessCommandHandler : IBaseHandler<UpdateAccessCommand, BaseMessageResponse<bool>>
{
    private readonly IPanelAccessControlRepository _repository;

    public UpdateAccessCommandHandler(IPanelAccessControlRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<bool>> Handle(UpdateAccessCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Title))
            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.WrongParameters);

        if (await _repository.AccessTitleExistsAsync(command.Title, exceptId: command.Id, cancellationToken))
            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DuplicateKey);

        var updated = await _repository.UpdateAccessAsync(new PanelMenuEntity
        {
            Id = command.Id,
            Title = command.Title,
            EnTitle = command.EnTitle,
            Url = command.Url,
            IconName = command.IconName,
            ParentId = command.ParentId,
            Description = command.Description,
            IsActive = command.IsActive,
        }, cancellationToken);

        if (!updated)
            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.NotFound);

        var response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully);
        response.Data = true;
        return response;
    }
}

public sealed record UpdateAccessCommand(
    int Id,
    string Title,
    string? EnTitle,
    string? Url,
    string? IconName,
    int? ParentId,
    string? Description,
    bool IsActive) : BaseCommand<BaseMessageResponse<bool>>;
