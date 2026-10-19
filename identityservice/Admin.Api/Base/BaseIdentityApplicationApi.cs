using Microsoft.Extensions.DependencyInjection;
using OmidBank.Common.Base;
using OmidBank.Common.Base.MediatR;
using OmidBank.Common.Controller.V2;
using OmidBank.Common.Models;

namespace Admin.Api;

public class BaseIdentityApplicationApi : BaseApplicationApi
{
    private IActiveTokenRepository _activeTokenRepository;
    private IUserRepository _userRepository;

    public override async Task<BaseMessageResponse<CurrentUserModel>> AuthenticateAsync(string activeToken, string mobileNumber, bool isNewUserWithMobileNumber, string identityServiceUrl, string apiKeyInternal)
    {
        _activeTokenRepository = HttpContext.RequestServices.GetService<IdentityService.Domain.IRepositories.IActiveTokenRepository>()!;
        _userRepository = HttpContext.RequestServices.GetService<IUserRepository>()!;

        var response = MessageResponse.GenerateMessageResponse<CurrentUserModel>(EResponseStatusCode.DoneSuccessfully);


        var status = await _activeTokenRepository.CheckTokenAsync(new ActiveTokenModel(activeToken, mobileNumber), default(CancellationToken));
        if (status != null)
        {
            if ((bool)status)
            {
                var user = await _userRepository.GetByMobileNumberAsync(mobileNumber, default(CancellationToken));
                if (!user.IsActive)
                    return MessageResponse.GenerateMessageResponse<CurrentUserModel>(EResponseStatusCode.Forbidden);

                response.Data = new CurrentUserModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    NationalCode = user.NationalCode,
                    BankCustomerNumber = user.BankCustomerNumber,
                    BirthDate = user.BirthDate??null,
                    
                };
                return response;
            }
            else
                return MessageResponse.GenerateMessageResponse<CurrentUserModel>(EResponseStatusCode.UnAuthorized);
        }
        else
        {
            return MessageResponse.GenerateMessageResponse<CurrentUserModel>(EResponseStatusCode.ErrorOccurred);
        }
    }

    public async Task<IActionResult> HandlePlainRequests<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken) where TRequest : BaseCommand<BaseMessageResponse<TResponse>>
    {
        request.CurrentUser = _baseRequest.CurrentUser;
        request.RequestInfo = _baseRequest.RequestInfo;
        var response = await _mediator.Send(request, cancellationToken);
        return BaseResult(response);
    }
}