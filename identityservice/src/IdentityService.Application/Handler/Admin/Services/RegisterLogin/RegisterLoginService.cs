using IdentityService.Contracts.Admin.CommandResponses;
using IdentityService.Domain.Models.User;
using IdentityService.Services.IServices;
using IdentityService.Services.ViewModels;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.InternalThirdParty.WalletService;
using OmidBank.Common.InternalThirdParty.WalletService.Request;

namespace IdentityService.Application.Handler.Admin.Services.RegisterLogin;

public class RegisterLoginService(
    IConfiguration configuration,
    ICityService cityService
    )
{
    private readonly bool _changePasswordWalletIsActive = bool.Parse(configuration["ChangePasswordWalletIsActive"]!);
    private readonly string _apiKeyInternal = configuration["ApiKeyInternal"]!;
    private readonly string _walletServiceUrl = configuration["WalletServiceUrl"]!;
    
    public async Task<LoginRegisterUserInformation> MakeResponse(UserModel user, CityInformationDTO city, string avatarUrl,string token, CancellationToken cancellationToken)
    {
        var changePasswordWallet = false;
        long? omidBankWalletAccountNumber = null;
        if (_changePasswordWalletIsActive)
        {
            var walletServiceProvider = new WalletServiceProvider(_walletServiceUrl, _apiKeyInternal);
            var walletInfoRequest = new WalletInfoRequest { UserId = user.Id, WalletTypeId = 1 };
            var findWalletInfo = await walletServiceProvider.WalletInfoAsync(walletInfoRequest, cancellationToken);
            if (findWalletInfo is not null && findWalletInfo.IsSuccess && findWalletInfo.Data is not null)
            {
                changePasswordWallet = findWalletInfo.Data.LastChangePassword is null && findWalletInfo.Data.Password is not null;
                omidBankWalletAccountNumber = findWalletInfo.Data.AccountNumber;
            }
        }
        return new LoginRegisterUserInformation(
            true, user.FirstName, user.LastName, user.CityId, city.CityTitle, city.CityEnTitle,
            city.ProvinceId, city.ProvinceTitle, city.ProvinceEnTitle, avatarUrl, 
            user.MobileNumber, token, user.InvitedCode, user.NationalCode,
            user.MyInviteCode!, changePasswordWallet, omidBankWalletAccountNumber, null,null, user.UserName);
    }
    
    public async Task<CityInformationDTO> GetCityInformation(int? cityId, CancellationToken cancellationToken)
    {
        if (cityId is null or 0)
            return new CityInformationDTO();
        return await cityService.GetCityById(cityId, cancellationToken);
    }
}