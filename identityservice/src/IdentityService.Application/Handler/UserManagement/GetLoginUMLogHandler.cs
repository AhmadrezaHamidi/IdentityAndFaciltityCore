using IdentityService.Domain.Entity.UserManagement.LoginLogOutHistory;
using IdentityService.Domain.Entity.UserManagement.UserManagementTokenLog;
using OmidBank.Common.Extentions;

namespace IdentityService.Application.Handler.UserManagement;

public class GetLoginUMLogHandler : IBaseHandler<GetLoginUMLogRequest, BaseMessageResponse<GetLoginUMLogResponse>>
{
    private readonly ILoginLogoutHistoryRepository _loginLogoutHistoryRepository;

    public GetLoginUMLogHandler(ILoginLogoutHistoryRepository loginLogoutHistoryRepository)
    {
        _loginLogoutHistoryRepository = loginLogoutHistoryRepository;
    }

    public async Task<BaseMessageResponse<GetLoginUMLogResponse>> Handle(GetLoginUMLogRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<GetLoginUMLogResponse>(EResponseStatusCode.DoneSuccessfully, new());
        response.Data.Items = (await _loginLogoutHistoryRepository.GetListByUserIdAsync(request.CurrentUser.Id, 20, cancellationToken))
            .Select(a => new GetLoginUMLogItem()
            {
                DeviceInfo = a.DeviceInfo,
                Ip = a.Ip,
                ActionType = a.TypeAction == 1 ? "ورود" : "خروج",
                ActionTypeInt = a.TypeAction,
                Status = a.Status == 1 ? true : false,
                CreateDateTime = a.CreateDate is not null ? a.CreateDate.Value.ToTimeStampSeconds() : null
            }).ToList();
        return response;
    }
}

public record GetLoginUMLogRequest() : BaseCommand<BaseMessageResponse<GetLoginUMLogResponse>>;

public class GetLoginUMLogResponse
{
    public List<GetLoginUMLogItem> Items { get; set; } = new();
}

public class GetLoginUMLogItem
{
    public string DeviceInfo { get; set; }
    public string Ip { get; set; }
    public string ActionType { get; set; }
    public int ActionTypeInt { get; set; }
    public long? CreateDateTime { get; set; }
    public bool? Status { get; set; }
}