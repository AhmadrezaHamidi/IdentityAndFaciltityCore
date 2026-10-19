using IdentityService.Domain.Entity.ContentManagement;
using IdentityService.Domain.Services.ContentManagement;
using OmidBank.Common.JwtHelper;
using static OmidBank.Common.Enums;

namespace IdentityService.Application.Handler.Admin.ContentManagement;

// ══════════════════════════════════════════════════════════════════════════
// «مدیریت محتوا» — چهار زیرمنو (بنرها، صفحات محتوایی، سوالات متداول،
// متن‌های اطلاع‌رسانی)، همه با یک فرم (عنوان/متن/تصویر/ترتیب نمایش/وضعیت).
// ══════════════════════════════════════════════════════════════════════════

public sealed record ContentItemResponse(
    int Id, string Category, string Title, string? Body, string? ImageUrl,
    int DisplayOrder, bool IsActive, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);

// ── فهرست ───────────────────────────────────────────────────────────────────
public sealed class ListContentItemsQueryHandler
    : IBaseHandler<ListContentItemsQuery, BaseMessageResponse<List<ContentItemResponse>>>
{
    private readonly IContentManagementRepository _repository;

    public ListContentItemsQueryHandler(IContentManagementRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<List<ContentItemResponse>>> Handle(
        ListContentItemsQuery query, CancellationToken cancellationToken)
    {
        if (!ContentCategory.Exists(query.Category))
            return MessageResponse.GenerateMessageResponse<List<ContentItemResponse>>(EResponseStatusCode.NotFound);

        var items = await _repository.ListAsync(query.Category, cancellationToken);

        var response = MessageResponse.GenerateMessageResponse<List<ContentItemResponse>>(EResponseStatusCode.DoneSuccessfully);
        response.Data = items.Select(ToResponse).ToList();
        return response;
    }

    private static ContentItemResponse ToResponse(ContentItemEntity i) =>
        new(i.Id, i.Category, i.Title, i.Body, i.ImageUrl, i.DisplayOrder, i.IsActive, i.CreatedAtUtc, i.UpdatedAtUtc);
}

public sealed record ListContentItemsQuery(string Category) : BaseQuery<BaseMessageResponse<List<ContentItemResponse>>>;

// ── ایجاد ───────────────────────────────────────────────────────────────────
public sealed class CreateContentItemCommandHandler : IBaseHandler<CreateContentItemCommand, BaseMessageResponse<int>>
{
    private readonly IContentManagementRepository _repository;
    private readonly IJwtService _jwtService;

    public CreateContentItemCommandHandler(IContentManagementRepository repository, IJwtService jwtService)
    {
        _repository = repository;
        _jwtService = jwtService;
    }

    public async Task<BaseMessageResponse<int>> Handle(CreateContentItemCommand command, CancellationToken cancellationToken)
    {
        if (!ContentCategory.Exists(command.Category) || string.IsNullOrWhiteSpace(command.Title))
            return MessageResponse.GenerateMessageResponse<int>(EResponseStatusCode.WrongParameters);

        int? panelUserId = null;
        if (!string.IsNullOrWhiteSpace(command.Token))
        {
            var valid = _jwtService.ValidateToken(command.Token);
            if (valid.Item1) panelUserId = (int)valid.Item3.UserId;
        }

        var response = MessageResponse.GenerateMessageResponse<int>(EResponseStatusCode.DoneSuccessfully);
        response.Data = await _repository.AddAsync(new ContentItemEntity
        {
            Category = command.Category,
            Title = command.Title,
            Body = command.Body,
            ImageUrl = command.ImageUrl,
            DisplayOrder = command.DisplayOrder,
            IsActive = command.IsActive,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByPanelUserId = panelUserId,
        }, cancellationToken);
        return response;
    }
}

public sealed record CreateContentItemCommand(
    string Category, string Title, string? Body, string? ImageUrl, int DisplayOrder, bool IsActive, string? Token)
    : BaseCommand<BaseMessageResponse<int>>;

// ── ویرایش ──────────────────────────────────────────────────────────────────
public sealed class UpdateContentItemCommandHandler : IBaseHandler<UpdateContentItemCommand, BaseMessageResponse<bool>>
{
    private readonly IContentManagementRepository _repository;

    public UpdateContentItemCommandHandler(IContentManagementRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<bool>> Handle(UpdateContentItemCommand command, CancellationToken cancellationToken)
    {
        if (!ContentCategory.Exists(command.Category) || string.IsNullOrWhiteSpace(command.Title))
            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.WrongParameters);

        var updated = await _repository.UpdateAsync(new ContentItemEntity
        {
            Id = command.Id,
            Category = command.Category,
            Title = command.Title,
            Body = command.Body,
            ImageUrl = command.ImageUrl,
            DisplayOrder = command.DisplayOrder,
            IsActive = command.IsActive,
        }, cancellationToken);

        if (!updated)
            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.NotFound);

        var response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully);
        response.Data = true;
        return response;
    }
}

public sealed record UpdateContentItemCommand(
    string Category, int Id, string Title, string? Body, string? ImageUrl, int DisplayOrder, bool IsActive)
    : BaseCommand<BaseMessageResponse<bool>>;

// ── فعال/غیرفعال ─────────────────────────────────────────────────────────────
public sealed class SetContentItemActiveCommandHandler : IBaseHandler<SetContentItemActiveCommand, BaseMessageResponse<bool>>
{
    private readonly IContentManagementRepository _repository;

    public SetContentItemActiveCommandHandler(IContentManagementRepository repository) => _repository = repository;

    public async Task<BaseMessageResponse<bool>> Handle(SetContentItemActiveCommand command, CancellationToken cancellationToken)
    {
        var updated = await _repository.SetActiveAsync(command.Category, command.Id, command.IsActive, cancellationToken);
        if (!updated)
            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.NotFound);

        var response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully);
        response.Data = true;
        return response;
    }
}

public sealed record SetContentItemActiveCommand(string Category, int Id, bool IsActive) : BaseCommand<BaseMessageResponse<bool>>;
