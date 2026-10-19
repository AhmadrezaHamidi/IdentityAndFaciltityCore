using IdentityService.Application.Handler.Admin.Services;
using IdentityService.Contracts.Admin.CommandResponses;
using IdentityService.Domain.IRepositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.Interface;
using OmidBank.Common.JwtHelper;
using System.Text.Json;
using OmidBank.Common.Base.Shared;
using OmidBank.Common.Cookie;

namespace IdentityService.Application.Handler.Admin.Commands.AdminVerifyLoginOtp;

public class AdminVerifyLoginOtpCommandHandler : IBaseHandler<AdminVerifyLoginOtpCommand, BaseMessageResponse<AdminVerifyLoginOtpDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAdminIdentityService _identityService;

    public AdminVerifyLoginOtpCommandHandler(IUserRepository userRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IAdminIdentityService identityService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<BaseMessageResponse<AdminVerifyLoginOtpDto>> Handle(AdminVerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        var response = await _identityService.AdminVerifyLoginOtp(request, cancellationToken);

        if (!response.IsSuccess)
            return response;

        var user = await _userRepository.GetPanelUserByMobileNumberAsync(request.RequestInfo.MobileNumber, cancellationToken);
        
        if (user == null)
        {
            return MessageResponse.GenerateMessageResponse<AdminVerifyLoginOtpDto>(EResponseStatusCode.NotFound)!;
        }

        List<RoleMenuModel> roleMenuModels = new List<RoleMenuModel>();

        var generateToken = _jwtService.GenerateAsync(
    user.Id,
                user.UserName,
                JsonSerializer.Serialize(user.RoleMenuModels.Select(o => o.RoleId).Distinct(), JsonHelper.JsonCamelCaseOption()),
                JsonSerializer.Serialize(user.RoleMenuModels.Select(o => o.RoleTitle).Distinct(), JsonHelper.JsonCamelCaseOption()),
                JsonSerializer.Serialize(Array.Empty<object>(), JsonHelper.JsonCamelCaseOption()));

        SetTokenCookie(generateToken);

        response.Data = new AdminVerifyLoginOtpDto(true, user.FirstName, user.LastName, null, null, null, null, null, null, null, user.MobileNumber, null, null, null, null, null, null, null, null, user.UserName);


        return response;
    }

    public void SetTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            // امنیت: جاوااسکریپت نتواند بخواند (ضد هک XSS)
            HttpOnly = true,

            // زمان انقضا
            Expires = DateTime.UtcNow.AddDays(1),

            // مسیر: در کل سایت معتبر باشد
            Path = "/",

            // --- تنظیمات حیاتی برای محیط توسعه (Localhost) ---
            // اگر پروژه شما https است (که معمولا هست):
            Secure = true,             // حتما true باشد
            SameSite = SameSiteMode.None // حتما None باشد تا مرورگر گیر ندهد
        };

        _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieKey.Token, token, cookieOptions);
    }
}