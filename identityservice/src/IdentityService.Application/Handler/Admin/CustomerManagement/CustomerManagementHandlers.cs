//using IdentityService.Application.Handler.Admin.UserManagementPanel;
//using IdentityService.Domain.Services.CustomerManagement;
//using OmidBank.Common.JwtHelper;
//using static OmidBank.Common.Enums;

//namespace IdentityService.Application.Handler.Admin.CustomerManagement;

//// ══════════════════════════════════════════════════════════════════════════
//// «مدیریت مشتریان» — سه زیرمنو: اطلاعات مشتری، مسدودسازی/فعال‌سازی حساب،
//// تغییر وضعیت دسترسی به هر خدمت (RBAC). PagedResult<T> از ماژول
//// UserManagementPanel بازاستفاده شده — همان شکل صفحه‌بندی است، دوباره تعریف نشد.
//// ══════════════════════════════════════════════════════════════════════════

//public sealed record CustomerListItem(
//    int UserId,
//    int PersonId,
//    string? FirstName,
//    string? LastName,
//    string? NationalCode,
//    string? MobileNumber,
//    bool BlockByAdmin);

//public sealed record ServiceAccessItem(string ServiceKey, bool IsEnabled, DateTime UpdatedAtUtc);

//// ── ۱. اطلاعات مشتری («فیلتر با کد ملی») ────────────────────────────────────
//public sealed class SearchCustomersQueryHandler
//    : IBaseHandler<SearchCustomersQuery, BaseMessageResponse<PagedResult<CustomerListItem>>>
//{
//    private readonly ICustomerManagementRepository _repository;

//    public SearchCustomersQueryHandler(ICustomerManagementRepository repository) => _repository = repository;

//    public async Task<BaseMessageResponse<PagedResult<CustomerListItem>>> Handle(
//        SearchCustomersQuery query, CancellationToken cancellationToken)
//    {
//        var (items, total) = await _repository.SearchCustomersAsync(query.NationalCode, query.Page, query.PageSize, cancellationToken);

//        var response = MessageResponse.GenerateMessageResponse<PagedResult<CustomerListItem>>(EResponseStatusCode.DoneSuccessfully);
//        response.Data = new PagedResult<CustomerListItem>(
//            items.Select(ToListItem).ToList(),
//            total, query.Page <= 0 ? 1 : query.Page, query.PageSize <= 0 ? 20 : query.PageSize);
//        return response;
//    }

//    private static CustomerListItem ToListItem(Domain.Entity.UserEntity u) => new(
//        u.Id, u.PersonId, u.Person?.FirstName, u.Person?.LastName, u.Person?.NationalCode, u.Person?.MobileNumber, u.BlockByAdmin);
//}

//public sealed record SearchCustomersQuery(string? NationalCode, int Page = 1, int PageSize = 20)
//    : BaseQuery<BaseMessageResponse<PagedResult<CustomerListItem>>>;

//// ── ۲. مسدودسازی / فعال‌سازی حساب کاربری ────────────────────────────────────
//public sealed class SetCustomerBlockStatusCommandHandler
//    : IBaseHandler<SetCustomerBlockStatusCommand, BaseMessageResponse<bool>>
//{
//    private readonly ICustomerManagementRepository _repository;

//    public SetCustomerBlockStatusCommandHandler(ICustomerManagementRepository repository) => _repository = repository;

//    public async Task<BaseMessageResponse<bool>> Handle(SetCustomerBlockStatusCommand command, CancellationToken cancellationToken)
//    {
//        var updated = await _repository.SetBlockStatusAsync(command.UserId, command.Blocked, cancellationToken);
//        if (!updated)
//            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.NotFound);

//        var response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully);
//        response.Data = true;
//        return response;
//    }
//}

//public sealed record SetCustomerBlockStatusCommand(int UserId, bool Blocked) : BaseCommand<BaseMessageResponse<bool>>;

//// ── ۳. تغییر وضعیت دسترسی به هر خدمت (RBAC) ─────────────────────────────────
//public sealed class GetCustomerServiceAccessQueryHandler
//    : IBaseHandler<GetCustomerServiceAccessQuery, BaseMessageResponse<List<ServiceAccessItem>>>
//{
//    private readonly ICustomerManagementRepository _repository;

//    public GetCustomerServiceAccessQueryHandler(ICustomerManagementRepository repository) => _repository = repository;

//    public async Task<BaseMessageResponse<List<ServiceAccessItem>>> Handle(
//        GetCustomerServiceAccessQuery query, CancellationToken cancellationToken)
//    {
//        var items = await _repository.GetServiceAccessAsync(query.UserId, cancellationToken);

//        var response = MessageResponse.GenerateMessageResponse<List<ServiceAccessItem>>(EResponseStatusCode.DoneSuccessfully);
//        response.Data = items.Select(a => new ServiceAccessItem(a.ServiceKey, a.IsEnabled, a.UpdatedAtUtc)).ToList();
//        return response;
//    }
//}

//public sealed record GetCustomerServiceAccessQuery(int UserId) : BaseQuery<BaseMessageResponse<List<ServiceAccessItem>>>;

//public sealed class SetCustomerServiceAccessCommandHandler
//    : IBaseHandler<SetCustomerServiceAccessCommand, BaseMessageResponse<bool>>
//{
//    private readonly ICustomerManagementRepository _repository;
//    private readonly IJwtService _jwtService;

//    public SetCustomerServiceAccessCommandHandler(ICustomerManagementRepository repository, IJwtService jwtService)
//    {
//        _repository = repository;
//        _jwtService = jwtService;
//    }

//    public async Task<BaseMessageResponse<bool>> Handle(SetCustomerServiceAccessCommand command, CancellationToken cancellationToken)
//    {
//        if (string.IsNullOrWhiteSpace(command.ServiceKey))
//            return MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.WrongParameters);

//        int? panelUserId = null;
//        if (!string.IsNullOrWhiteSpace(command.Token))
//        {
//            var valid = _jwtService.ValidateToken(command.Token);
//            if (valid.Item1) panelUserId = (int)valid.Item3.UserId;
//        }

//        await _repository.SetServiceAccessAsync(command.UserId, command.ServiceKey, command.IsEnabled, panelUserId, cancellationToken);

//        var response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully);
//        response.Data = true;
//        return response;
//    }
//}

//public sealed record SetCustomerServiceAccessCommand(int UserId, string ServiceKey, bool IsEnabled, string? Token)
//    : BaseCommand<BaseMessageResponse<bool>>;
