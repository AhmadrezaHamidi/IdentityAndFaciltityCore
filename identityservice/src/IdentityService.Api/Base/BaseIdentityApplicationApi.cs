using Microsoft.Extensions.DependencyInjection;
using OmidBank.Common.Models;

namespace IdentityService.Api;

public class BaseIdentityApplicationApi : BaseApplicationApi
{
    private IActiveTokenRepository _activeTokenRepository;
    private IUserRepository _userRepository;

    public override async Task<BaseMessageResponse<CurrentUserModel>> AuthenticateAsync(string activeToken, string mobileNumber, bool isNewUserWithMobileNumber, string identityServiceUrl, string apiKeyInternal)
    {
        _activeTokenRepository = HttpContext.RequestServices.GetService<IActiveTokenRepository>()!;
        _userRepository = HttpContext.RequestServices.GetService<IUserRepository>()!;

        var response = MessageResponse.GenerateMessageResponse<CurrentUserModel>(EResponseStatusCode.DoneSuccessfully);

        var status = await _activeTokenRepository.CheckTokenAsync(new ActiveTokenModel(activeToken, mobileNumber), default);
        if (isNewUserWithMobileNumber && (status == null || !(bool)status))
        {
            var checkActiveTokenBeforeLogin = new ActiveTokenModelBeforeLogin(activeToken, mobileNumber);
            var token = await _activeTokenRepository.RetrieveTokenBeforeLoginAsync(checkActiveTokenBeforeLogin, default);
            if (token is not null)
            {
                status = true;
                mobileNumber = token.OldMobileNumber;
            }
        }
        if (status != null)
        {
            if ((bool)status)
            {
                var user = await _userRepository.GetByMobileNumberAsync(mobileNumber, default(CancellationToken));
                if (!user.IsActive)
                    return MessageResponse.GenerateMessageResponse<CurrentUserModel>(EResponseStatusCode.Forbidden);

                //var userIsActive = await _userRepository.IsActive(user.Id, default(CancellationToken));
                //if (userIsActive)
                //    return MessageResponse.GenerateMessageResponse<CurrentUserModel>(EResponseStatusCode.Forbidden);
                response.Data = new CurrentUserModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    NationalCode = user.NationalCode,
                    BankCustomerNumber = user.BankCustomerNumber,
                    BirthDate = user.BirthDate ?? null,

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
}