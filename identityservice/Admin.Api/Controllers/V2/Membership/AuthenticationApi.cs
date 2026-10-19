using IdentityService.Application.Handler.Admin;
using IdentityService.Application.Handler.Admin.Commands.AdminLoginWithPhoneNumber;
using IdentityService.Application.Handler.Admin.Commands.AdminVerifyLoginOtp;
using IdentityService.Contracts.Admin.CommandResponses;
using Microsoft.AspNetCore.Mvc;
using OmidBank.Common.Attribute;
using OmidBank.Common.Attribute.V2;
using OmidBank.Common.Interface;
using Swashbuckle.AspNetCore.Annotations;
using static IdentityService.Application.Handler.Admin.LoginHandler;


namespace Admin.Api.Controllers.V1.Membership;

[Route("Authentication")]
[ApiVersion("2")]
public class AuthenticationApi : BaseIdentityApplicationApi//BaseAdminApi
{
    public AuthenticationApi()
    {
    }

    [HttpPost("Login")]
    [Anonymous]
    public async Task<IActionResult> Login([FromBody] LoginQuery query)
    {
        var response = await _mediator.Send(query);
        return BaseResult(response);
    }

    [AnonymousAttribute]
    [HttpGet("Generate")]
    public async Task<IActionResult> Generate([FromQuery] GenerateCaptchaQuery request, CancellationToken cancellationToken)
    {
        return BaseResult(await _mediator.Send(request, cancellationToken));
    }


    [HttpPost("MainMenu")]
    [Anonymous]
    public async Task<IActionResult> MainMenu()
    {
        var response = await _mediator.Send(new MainMenuQuery(_baseRequest.RequestInfo.Token));
        return BaseResult(response);
    }
    [HttpGet("Logout")]
    [AuthenticationUserAdminByCookie]
    public async Task<IActionResult> LogOut([FromQuery] LogoutQuery query)
        => BaseResult(await _mediator.Send(query));

}