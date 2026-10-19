using Amazon.Runtime.Internal.Transform;
using IdentityService.Contracts.Transaction;
using IdentityService.Domain.IRepositories;
using IdentityService.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OmidBank.Common.Base.Shared;
using OmidBank.Common.Cache;
using OmidBank.Common.Cookie;
using OmidBank.Common.Cryptography;
using OmidBank.Common.Interface;
using OmidBank.Common.JwtHelper;
using System.Security.Cryptography;
using System.Text.Json;
using XAct;
using static OmidBank.Common.Enums;

namespace IdentityService.Application.Handler.Admin;

public sealed class LoginHandler : IBaseHandler<LoginQuery, BaseMessageResponse<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IAdminHandlerUtility _handlerUtility;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly IActiveTokenRepository activeTokenRepository;
    private readonly IUnitOfWork unitOfWork;

    public LoginHandler(IConfiguration configuration, IUserRepository userRepository, IJwtService jwtService,
        IAdminHandlerUtility handlerUtility, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor,
        IRedisCacheManager redisCacheManager, IActiveTokenRepository activeTokenRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _handlerUtility = handlerUtility;
        _env = env;
        _httpContextAccessor = httpContextAccessor;
        _redisCacheManager = redisCacheManager;
        this.activeTokenRepository = activeTokenRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<BaseMessageResponse<LoginResponse>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        // ۱. بررسی کپچا — قبلاً اصلاً چک نمی‌شد
        var captchaKey = string.Format(CacheKey.AdminCaptcha, query.CaptchaToken);
        var storedCaptcha = _redisCacheManager.Read(captchaKey); // یک‌بارمصرف: بعد پاک می‌شه

        if (string.IsNullOrEmpty(storedCaptcha) ||
            !string.Equals(storedCaptcha, query.CaptchaValue, StringComparison.OrdinalIgnoreCase))
        {
            return MessageResponse.GenerateMessageResponse<LoginResponse>(EResponseStatusCode.InvalidPassword);
            // اگر مقدار جدا برای کپچای غلط تو enum مشترک دارید (مثلاً InvalidCaptcha) به‌جاش استفاده کنید
        }

        _redisCacheManager.Remove(captchaKey); // جلوگیری از استفادهٔ دوبارهٔ همون کپچا

        // ۲. خواندن کاربر با EF (بدون SP)
        var user = await _userRepository.CheckPanelUserByUserNameAsync(query.Username, cancellationToken);


        if (user == null || !OmidBank.Common.Helper.Hashing.Cryptography.Verify(query.Password, user.Password))
            return MessageResponse.GenerateMessageResponse<LoginResponse>(EResponseStatusCode.InvalidPassword);


        var roleMenuClaimPayload = user.RoleMenuModels
            .Where(o => !string.IsNullOrEmpty(o.MenuUrl))
            .Select(o => new { MenuUrl = o.MenuUrl })
            .Distinct()
            .ToList();

        var generateToken = _jwtService.GenerateAsync(
            user.Id,
            user.UserName,
            JsonSerializer.Serialize(user.RoleMenuModels.Select(o => o.RoleId).Distinct(), JsonHelper.JsonCamelCaseOption()),
            JsonSerializer.Serialize(user.RoleMenuModels.Select(o => o.RoleTitle).Distinct(), JsonHelper.JsonCamelCaseOption()),
            JsonSerializer.Serialize(roleMenuClaimPayload, JsonHelper.JsonCamelCaseOption()));

        var activeToken = ActiveTokenModel.CreateToken(
           token: generateToken,
           mobileNumber: user.MobileNumber,
           deviceInfo: query.RequestInfo.DeviceInfo,
           version: query.RequestInfo.Version,
           ip: _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString(),
                timeStamp: DateTimeOffset.Now.AddDays(1).ToUnixTimeMilliseconds()
            );


        await activeTokenRepository.InsertTokenAsync(activeToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);



        SetTokenCookie(generateToken);

        var response = MessageResponse.GenerateMessageResponse<LoginResponse>(EResponseStatusCode.DoneSuccessfully, new LoginResponse
        {
            Token = generateToken
        });

        return response;
    }

    public async Task SetTokenCookie(string token)
    {
        var isHttps = _httpContextAccessor.HttpContext.Request.IsHttps;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.AddDays(1),
            Path = "/",
            // روی HTTPS واقعی: Secure + SameSite=None (برای cross-site).
            // روی HTTP توسعه (localhost): Secure باید false باشه وگرنه مرورگر
            // کوکی رو اصلاً ذخیره نمی‌کنه؛ SameSite=None بدون Secure هم رد می‌شه،
            // پس با هم به Lax سوییچ می‌کنیم (کافیه چون Vite همیشه same-origin پروکسی می‌کنه).
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax
        };

        _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieKey.Token, token, cookieOptions);
        _redisCacheManager.Write(string.Format(CacheKey.TosanUserSessionId, token), token, Enums.ECacheTime.OneDay); 
    }
}






public sealed record LoginQuery([Required] string Username, [Required] string Password, [Required] Guid CaptchaToken, [Required] string CaptchaValue) : BaseQuery<BaseMessageResponse<LoginResponse>>;

public sealed class LoginResponse
{
    public string? Token { get; set; }
    public string OtpTracker { get; set; }
    public DateTime? OtpExpireTime { get; set; }
}

