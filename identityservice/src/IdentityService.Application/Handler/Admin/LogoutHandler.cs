using System.Text.Json;
using IdentityService.Domain.IRepositories;
using IdentityService.Domain.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.Base.Shared;
using OmidBank.Common.Cache;
using OmidBank.Common.Cookie;
using OmidBank.Common.Cryptography;
using OmidBank.Common.Interface;
using OmidBank.Common.JwtHelper;
using static OmidBank.Common.Enums;

namespace IdentityService.Application.Handler.Admin;

public sealed class LogoutHandler : IBaseHandler<LogoutQuery, BaseMessageResponse<LogoutResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IActiveTokenRepository _activeTokenRepository;
    private readonly IAdminHandlerUtility _handlerUtility;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly IJwtService _jwtService;
    private readonly string _jwtTokenEncryptorKey;

    public LogoutHandler(IConfiguration configuration, IUserRepository userRepository, IJwtService jwtService, IAdminHandlerUtility handlerUtility, IRedisCacheManager redisCacheManager, IActiveTokenRepository activeTokenRepository)
    {
        _activeTokenRepository = activeTokenRepository;
        _userRepository = userRepository;
        _jwtService = jwtService;
        _jwtTokenEncryptorKey = configuration["JwtTokenEncryptorKey"];
        _handlerUtility = handlerUtility;
        _redisCacheManager = redisCacheManager;
    }

    public async Task<BaseMessageResponse<LogoutResponse>> Handle(LogoutQuery query, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<LogoutResponse>(EResponseStatusCode.DoneSuccessfully, new());
        
        var cookieToken = _handlerUtility.ReadCookie(CookieKey.Token);

        await _activeTokenRepository.DeActiveTokenAsync(cookieToken, cancellationToken);
        _redisCacheManager.Remove(string.Format(CacheKey.TosanUserSessionId, cookieToken));

        return response;
    }
}

public sealed record LogoutQuery() : BaseQuery<BaseMessageResponse<LogoutResponse>>;

public sealed class LogoutResponse
{
}