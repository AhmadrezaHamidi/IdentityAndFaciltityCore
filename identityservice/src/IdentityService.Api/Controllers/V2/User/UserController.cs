using IdentityService.Application.Handler.User;
using IdentityService.Domain.Entity.ChangeMobileNumber;

namespace IdentityService.Api.Controllers.V2.User;


[ApiVersion("2")]
public class UserController : BaseIdentityApplicationApi
{
    [HttpPost]
    public async Task<IActionResult> AutoModifyCustomerNumber([FromBody] AutoModifyCustomerNumberRequest command, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(command, cancellationToken));
    
    [HttpGet]
    public async Task<IActionResult> UserInfo([FromQuery] GetUserInfoRequest query, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(query, cancellationToken));

    [HttpPost("/v{version:apiVersion}/[controller]/mobile-number-change")]
    [UserWithNewMobileNumber]
    public async Task<IActionResult> MakeChangeMobileNumber([FromBody] MakeChangeMobileNumberRequest request, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(request, cancellationToken));
    
    [HttpPost("/v{version:apiVersion}/[controller]/mobile-number-change/agreements")]
    [UserWithNewMobileNumber]
    public Task<IActionResult> ChangeMobileNumberAgreements(IOptions<ChangeMobileNumberOptions> options)
    {
        var response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully);
        response.Data = new { options.Value.Agreements };
        return Task.FromResult(BaseResult(response));
    }

    [HttpPost("/v{version:apiVersion}/[controller]/mobile-number-change/introduction")]
    [UserWithNewMobileNumber]
    public Task<IActionResult> ChangeMobileNumberIntroduction(IOptions<ChangeMobileNumberOptions> options)
    {
        // options.Value.Introduction.PhoneNumber = _baseRequest.RequestInfo.MobileNumber;
        return Task.FromResult(BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, options.Value.Introduction)));
    }
    
    [HttpPost("/v{version:apiVersion}/[controller]/mobile-number-change/result")]
    public async Task<IActionResult> ChangeMobileNumberResult([FromBody] ChangeMobileNumberResultRequest request, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(request, cancellationToken));
    
    [HttpPost("/v{version:apiVersion}/[controller]/mobile-number-change/wage")]
    [UserWithNewMobileNumber]
    public Task<IActionResult> ChangeMobileNumberWage(IOptions<ChangeMobileNumberOptions> options)
    {
        var response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully);
        response.Data = options.Value.EnableWage ?
            new ChangeMobileNumberPaymentResponse(options.Value.Wage, options.Value.Wage) :
            new ChangeMobileNumberPaymentResponse(0, 0);
        return Task.FromResult(BaseResult(response));
    }
    
    [HttpPost("/v{version:apiVersion}/[controller]/mobile-number-change/otp/send")]
    public async Task<IActionResult> ChangeMobileNumberSendOtp([FromBody] ChangeMobileNumberSendOtpRequest request, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(request, cancellationToken));
    
    [HttpPost("/v{version:apiVersion}/[controller]/mobile-number-change/otp/verify")]
    public async Task<IActionResult> ChangeMobileNumberVerifyOtp([FromBody] ChangeMobileNumberVerifyOtpRequest request, CancellationToken cancellationToken)
        => BaseResult(await _mediator.Send(request, cancellationToken));
}