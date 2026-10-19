using MediatR;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using OmidBank.Common.Base.V2;
using OmidBank.Common.Controller.V2;
using OmidBank.Common.Cookie;
using OmidBank.Common.Interface;

[ApiController]
[Route("[controller]/[action]")]
public class BaseAdminApi : BaseApi
{
    protected IMediator _mediator;
    //protected IAdminPanelUserReadRepository _adminPanelUserReadRepository;
    protected IAdminHandlerUtility _handlerUtility;

    protected BaseRequest _baseRequest { get; set; }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
        _handlerUtility = HttpContext.RequestServices.GetService<IAdminHandlerUtility>()!;

        _baseRequest = new BaseRequest
        {
            RequestInfo = CommonHelper.GetRequestHeaders(context.HttpContext)
        };

        // ⚠️ فیکس: اگه هدر Authorization نبود (فرانت فقط کوکی می‌فرسته)، از کوکی بخون
        if (string.IsNullOrWhiteSpace(_baseRequest.RequestInfo.Token))
        {
            var cookieToken = context.HttpContext.Request.Cookies[CookieKey.Token];
            if (!string.IsNullOrWhiteSpace(cookieToken))
            {
                _baseRequest.RequestInfo.Token = cookieToken;
                // اگه Token فقط init/readonly بود (خطای کامپایل گرفتی)، به‌جای خط بالا این رو بذار:
                // _baseRequest.RequestInfo = _baseRequest.RequestInfo with { Token = cookieToken };
            }
        }

        // _adminPanelUserReadRepository = HttpContext.RequestServices.GetService<IAdminPanelUserReadRepository>()!;

        // if (!IsAnonymousAction(context) && (!_baseRequest.IsLogin || !IsActiveToken()))
        // {
        //     context.Result = AccessDenyResult(_handlerUtility);
        //     return;
        // }
        //
        // if (!IsAnonymousAction(context) && !SkipForceChangePassword(context) && ForceChangePassword())
        // {
        //     context.Result = BaseResult(_handlerUtility.GenerateResponse(EBaseMessageResponseStatusCode.ForceChangePassword));
        //     return;
        // }

        base.OnActionExecuting(context);
    }

    // private bool ForceChangePassword()
    // {
    //     var settingValue = _handlerUtility.GetSettingValue(ESettingKey.AdminForceChangePasswordDay);
    //
    //     if (!int.TryParse(settingValue, out var forceChangeDays) || forceChangeDays <= 0)
    //         return false;
    //
    //     var lastChangePasswordTime = _adminPanelUserReadRepository.GetLastChangePasswordTime(_baseRequest.CurrentUser.UserId);
    //
    //     if (lastChangePasswordTime is null)
    //         return true;
    //
    //     var daysSinceChange = (DateTime.Now - lastChangePasswordTime.Value).TotalDays;
    //     return daysSinceChange >= forceChangeDays;
    // }

    private bool IsActiveToken()
    {
        // var readToken = _handlerUtility.ReadCookie(CookieKeyConstant.Token);
        // if (string.IsNullOrWhiteSpace(readToken))
        //     return false;
        //
        //
        // string activeTokenKey = string.Format(CacheKey.AdminActiveToken, _baseRequest.CurrentUser.UserId);
        // var readCacheKey = _handlerUtility.ReadCache<string>(activeTokenKey);
        // if (string.IsNullOrWhiteSpace(readCacheKey))
        //     return false;
        //
        // if (readCacheKey != readToken)
        //     return false;

        return true;
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (_baseRequest.IsLogin && !SkipRefreshTokenAttribute(context))
        {
            //   RefreshToken();
        }

        base.OnActionExecuted(context);
    }

    protected bool SkipRefreshTokenAttribute(FilterContext context)
    {
        return context.ActionDescriptor is ControllerActionDescriptor descriptor &&
               descriptor.MethodInfo.GetCustomAttributes(inherit: true)
                   .Any(attr => attr.GetType() == typeof(IgnoreRefreshTokenAttribute));
    }

    //     private void RefreshToken()
    //     {
    //         var jwtProvider = JwtProvider.GetInstance(_handlerUtility.GetSettingValue(ESettingKey.JWtSecretKey));
    //         string token = jwtProvider.CreateToken(
    //             _baseRequest.CurrentUser.UserId,
    //             _baseRequest.CurrentUser.UserName,
    //             _baseRequest.CurrentUser.FirstName,
    //             _baseRequest.CurrentUser.LastName
    //         );
    //         string activeTokenKey = string.Format(CacheKey.AdminActiveToken, _baseRequest.CurrentUser.UserId);
    //         _handlerUtility.WriteCache(activeTokenKey, token, Enums.ECacheTime.OneDay);
    //         _handlerUtility.WriteCookie(new CookieWriteRequest
    //         {
    //             Key = CookieKeyConstant.Token,
    //             Value = token,
    //             IsHttpOnly = true,
    //             SameSiteMode = SameSiteMode.Strict,
    //             ExpireTime = ECookieTime.OneDay
    //         });
    //     }
    public class IgnoreRefreshTokenAttribute : System.Attribute
    {
    }
}