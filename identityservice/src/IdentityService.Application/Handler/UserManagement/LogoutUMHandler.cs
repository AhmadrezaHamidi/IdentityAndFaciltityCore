using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.UserManagement.LoginLogOutHistory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmidBank.Common.Cache;
using OmidBank.Common.ExternalThirdParty.Tosan.User;
using OmidBank.Common.ExternalThirdParty.Tosan.User.Request;
using OmidBank.Common.Middleware;

namespace IdentityService.Application.Handler.UserManagement;
public class LogoutUMHandler : IBaseHandler<LogoutUMRequest, BaseMessageResponse<LogoutUMResponse>>
{
    private readonly IUserManagementRepository _userManagementRepository;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly IConfiguration _configuration;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly TosanUserProvider _tosanUserProvider;
    private readonly ILoginLogoutHistoryRepository _loginLogoutHistoryRepository;

    public LogoutUMHandler(IUserManagementRepository userManagementRepository, ILogger<RequestResponseLoggingMiddleware> logger
         ,IConfiguration configuration, IRedisCacheManager redisCacheManager, TosanUserProvider tosanUserProvider, ILoginLogoutHistoryRepository loginLogoutHistoryRepository)
    {
        _userManagementRepository = userManagementRepository;
        _logger = logger;
        _configuration = configuration;
        _redisCacheManager = redisCacheManager;
        _tosanUserProvider = tosanUserProvider;
        _loginLogoutHistoryRepository = loginLogoutHistoryRepository;
    }

    public async Task<BaseMessageResponse<LogoutUMResponse>> Handle(LogoutUMRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<LogoutUMResponse>(EResponseStatusCode.DoneSuccessfully, null);
        //var checkExistUser = _userManagementRepository.CheckExistByUserId(request.CurrentUser.Id);
        //if (checkExistUser != true)
        //    return MessageResponse.GenerateMessageResponse<LogoutUMResponse>(EResponseStatusCode.BadRequest);
        //if (request.MobileNumber != request.RequestInfo.MobileNumber)
        //    return MessageResponse.GenerateMessageResponse<LogoutUMResponse>(EResponseStatusCode.BadRequest);
        var resultLogout = _tosanUserProvider.LogoutAsync(new  LogoutRequest(request.CurrentUser.TosanToken), cancellationToken);
        if (resultLogout.Exception is not null)
        {
            response = MessageResponse.GenerateMessageResponse<LogoutUMResponse>(EResponseStatusCode.CallErrorTosan);
            return response;
        }
        _redisCacheManager.Remove(string.Format(CacheKey.TosanUserSessionId, request.RequestInfo.Token));
        //var userMangementData = await _userManagementRepository.GetByUserIdAsync(request.CurrentUser.Id, cancellationToken);
        //await _userManagementBiometricRepository.DeActiveBiometricToken(userMangementData.Id, cancellationToken);
        //await _userManagementRepository.LogoutUM(request.MobileNumber, cancellationToken);
        await _loginLogoutHistoryRepository.AddAsync(new()
        {
            Status = 1,
            TypeAction =2,
            Ip = request.RequestInfo.IP,
            DeviceInfo =request.RequestInfo.DeviceInfo,
            UserId = request.CurrentUser!.Id,
        }, cancellationToken);
        await _loginLogoutHistoryRepository.SaveChangeAsync(cancellationToken);
        return response;
    }

}

public record LogoutUMRequest([Required] string MobileNumber) : BaseCommand<BaseMessageResponse<LogoutUMResponse>>;

public class LogoutUMResponse { }

