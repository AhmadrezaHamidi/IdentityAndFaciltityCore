using IdentityService.Application.Handler.Identities.Commands.ChangeUserActiveState.DeactiveUserState;
using IdentityService.Application.Handler.Identities.Commands.ChangeUserActiveState.VerifyChangeUserStateOtp;
using IdentityService.Application.Handler.Identities.Commands.LoginWithPhoneNumber;
using IdentityService.Application.Handler.Identities.Commands.RegisterUser;
using IdentityService.Application.Handler.Identities.Commands.VerifyLoginOtp;
using IdentityService.Contracts.Identities.CommandResponses;
using IdentityService.Contracts.Services.BruteForceDetection;
using IdentityService.Contracts.Services.BruteForceDetection.Enums;
using OmidBank.Common.Base.MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityService.Api.Controllers.V3;

[ApiVersion("3")]
[Route("v{version:apiVersion}/[controller]")]
public class IdentityController(IBruteForceDetection bruteForceDetection) : BaseIdentityApplicationApi
{
    [HttpPost("login")]
    [Anonymous]
    [SwaggerOperation(Summary = "وب سرویس ورود به برنامه")]
    public async Task<IActionResult> LoginWithPhoneNumber([FromBody] LoginWithPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        return await HandlePlainRequests<LoginWithPhoneNumberCommand, LoginWithPhoneNumberDto>(request, cancellationToken);
    }

    [HttpPost("login/otp/verify")]
    [Anonymous]
    [SwaggerOperation(Summary = "وب سرویس بررسی رمز یکبار مصرف")]
    public async Task<IActionResult> VerifyLoginOtp([FromBody] VerifyLoginOtpCommand request, CancellationToken cancellationToken)
    {
        var bruteForceStatus = await bruteForceDetection.IsBruteForceDetectedAsync(request.RequestInfo.MobileNumber, request.RequestInfo.IP, cancellationToken);
        if (bruteForceStatus is not EBruteForceStatus.Ok)
            return BaseResult(MessageResponse.GenerateMessageResponse<VerifyLoginOtpDto>(EResponseStatusCode.TooManyRequests));

        return await HandlePlainRequests<VerifyLoginOtpCommand, VerifyLoginOtpDto>(request, cancellationToken);
    }

    [HttpPost("register")]
    [Anonymous]
    [SwaggerOperation(Summary = "وب سرویس ثبت نام کاربر در سیستم")]
    public async Task<IActionResult> RegisterUserCommand([FromBody] RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await HandlePlainRequests<RegisterUserCommand, RegisterUserDto>(request, cancellationToken);
    }

    [HttpPost("user/state/deactive")]
    [SwaggerOperation(Summary = "وب سرویس غیرفعال کردن حساب کاربر")]
    public async Task<IActionResult> DeactiveUserState([FromBody] DeactiveUserStateCommand request, CancellationToken cancellationToken)
    {
        return await HandlePlainRequests<DeactiveUserStateCommand, object>(request, cancellationToken);
    }

    [HttpPost("deactive/otp/verify")]
    [SwaggerOperation(Summary = "وب سرویس بررسی رمز یکبار مصرف برای غیرفعال کردن حساب کاربر")]
    public async Task<IActionResult> VerifyOtpDeactiveUserState([FromBody] VerifyOtpDeactiveUserStateCommand request, CancellationToken cancellationToken)
    {
        return await HandlePlainRequests<VerifyOtpDeactiveUserStateCommand, object>(request, cancellationToken);
    }

    [HttpGet("deactive/user/agreements")]
    [SwaggerOperation(Summary = "وب سرویس دریافت قوانین و مقررات غیرفعال کردن حساب کاربر")]
    public async Task<IActionResult> Agreements(CancellationToken cancellationToken)
    {
        var agreements = await System.IO.File.ReadAllTextAsync("./ChangeUserActiveStateAgreements.txt", cancellationToken);
        var response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, agreements);
        return BaseResult(response);
    }

    private async Task<IActionResult> HandlePlainRequests<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken) where TRequest : BaseCommand<BaseMessageResponse<TResponse>>
    {
        request.CurrentUser = _baseRequest.CurrentUser;
        request.RequestInfo = _baseRequest.RequestInfo;
        var response = await _mediator.Send(request, cancellationToken);
        return BaseResult(response);
    }
}