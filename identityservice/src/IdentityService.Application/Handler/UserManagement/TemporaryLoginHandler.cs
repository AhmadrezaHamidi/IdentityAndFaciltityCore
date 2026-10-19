using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmidBank.Common.Cache;
using OmidBank.Common.ExternalThirdParty.Tosan.User;
using OmidBank.Common.ExternalThirdParty.Tosan.User.Request;

namespace IdentityService.Application.Handler.UserManagement;

public class TemporaryLoginHandler : IBaseHandler<TemporaryLoginRequest, BaseMessageResponse<TemporaryLoginResponse>>
{
    private readonly bool _isDevelopTosan;
    private readonly string _baseUrl;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly int _redisCacheExpireTimeForLoginTemporary;
    private readonly ILogger<TemporaryLoginHandler> _logger;
    private readonly TosanUserProvider _tosanUserProvider;


    public TemporaryLoginHandler(IConfiguration configuration, IRedisCacheManager redisCacheManager, ILogger<TemporaryLoginHandler> logger, TosanUserProvider tosanUserProvider)
    {
        _redisCacheManager = redisCacheManager;
        _redisCacheExpireTimeForLoginTemporary = int.Parse(configuration["RedisCacheExpireTimeForLoginTemporary"] ?? "0");
        _isDevelopTosan = bool.Parse(configuration["IsDevelopTosan"] ?? "false");
        _baseUrl = configuration["BaseUrlTosan"];
        _logger = logger;
        _tosanUserProvider = tosanUserProvider;
    }

    public async Task<BaseMessageResponse<TemporaryLoginResponse>> Handle(TemporaryLoginRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<TemporaryLoginResponse>(EResponseStatusCode.DoneSuccessfully, null);
        response.Data = new TemporaryLoginResponse();

        if (_isDevelopTosan)
        {
            _logger.LogError("isDevelopTosan: " + _isDevelopTosan);
            response.Data.UserIsNew = false;
            response.Data.UserIsNew = request.Cvv2 == "2222";
            response.Data.Cif = "123";
            return response;
        }

        var systemSessionId = _redisCacheManager.Read(string.Format(CacheKey.TosanSystemSessionId));

        if (systemSessionId is null)
        {
            response = MessageResponse.GenerateMessageResponse<TemporaryLoginResponse>(EResponseStatusCode.UnAuthorized);
            return response;
        }
        _logger.LogError("_baseUrl: " + _baseUrl);

        _logger.LogError("sessionId: " + systemSessionId);

        var CardLoginRequest = new CardLoginWithTemporarySessionRequest(systemSessionId.ToString())
        {
            Pan = request.Pan,
            Pin = request.Pin,
            ExpDate = request.ExpDate,
            Cvv2 = request.Cvv2
        };

        _logger.LogError("CardLoginRequest: " + JsonSerializer.Serialize(CardLoginRequest));
        var result = await _tosanUserProvider.CardLoginWithTemporarySessionAsync(CardLoginRequest, cancellationToken);

        _logger.LogError("result: " + JsonSerializer.Serialize(result));

        if (result.Exception is not null)
        {
            _logger.LogError("result.Exception: " + JsonSerializer.Serialize(result.Exception));
            response = MessageResponse.GenerateMessageResponse<TemporaryLoginResponse>(EResponseStatusCode.CallErrorTosan);
            response.Message = result.Exception.TranslateMessage;
            return response;
        }

        _logger.LogError("redisCacheExpireTimeForLoginTemporary: " + JsonSerializer.Serialize(_redisCacheExpireTimeForLoginTemporary));
        if (_redisCacheExpireTimeForLoginTemporary != 0)
        {
            _redisCacheManager.Write(string.Format(CacheKey.TosanLoginTemporaryUserSessionId, request.RequestInfo.Token), result.Result.CookieSessionId, _redisCacheExpireTimeForLoginTemporary);
        }
        else
        {
            _redisCacheManager.Write(string.Format(CacheKey.TosanLoginTemporaryUserSessionId, request.RequestInfo.Token), result.Result.CookieSessionId, Enums.ECacheTime.AnHour);
        }

        response.Data.UserIsNew = result.Result.UserIsNew ?? false;
        response.Data.Cif = result.Result.Cif;
        _logger.LogError(" response.Data: " + JsonSerializer.Serialize(response.Data));
        return response;
    }
}

public record TemporaryLoginRequest(string ExpDate, string Pan, string Pin, string Cvv2) : BaseCommand<BaseMessageResponse<TemporaryLoginResponse>>;

public class TemporaryLoginResponse
{
    public bool UserIsNew { get; set; }
    public string Cif { get; set; }
}