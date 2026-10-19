using AutoMapper;
using IdentityService.Application.Handler.Identities.Commands.RegisterUser;
using IdentityService.Application.Handler.Identities.Services.RegisterLogin;
using IdentityService.Application.Handler.User;
using IdentityService.Application.Services.ChangeMobileNumber;
using IdentityService.Contracts.Identities.CommandResponses;
using IdentityService.Contracts.Identities.Repositories.ActiveTokens;
using IdentityService.Domain.IRepositories;
using IdentityService.Domain.Models;
using IdentityService.Domain.Models.User;
using IdentityService.Services.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OmidBank.Common.Cache;
using OmidBank.Common.Extentions;
using OmidBank.Common.InternalThirdParty.ShahkarService;
using OmidBank.Common.InternalThirdParty.ShahkarService.Request;
using OmidBank.Common.JwtHelper;
using System.Text.Json;

namespace IdentityService.Application.Handler.Identities.Services.Register;

public class RegisterService(
    IUserRepository userRepository,
    IPersonTempRepository personTempRepository,
    IActiveTokenCommandRepository activeTokenCommandRepository,
    RegisterLoginService registerLoginService,
    IMapper mapper,
    IConfiguration configuration,
    IOptions<ShahkarServiceProvider.ShahkarProviderOptions> shahkarOptions,
    ILogger<RegisterService> logger,
    IRequestHelper requestHelper,
    IRedisCacheManager redisCacheManager,
    IJwtService jwtService,
    IChangeMobileNumberService changeMobileNumberService,
    HttpClient httpClient)
{
    private readonly string _apiKeyInternal = configuration["ApiKeyInternal"]!;
    private readonly string _walletServiceUrl = configuration["WalletServiceUrl"]!;

    public async Task<BaseMessageResponse<RegisterUserDto>> RegisterUser(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.MobileNumber) ||
           string.IsNullOrEmpty(request.NationalCode) || string.IsNullOrEmpty(request.RegisterToken))
                return MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.BadRequest)!;

            UserModel userData = new();
            string token;

            var person = await userRepository.GetPersonByMobileNumber(request.MobileNumber, cancellationToken);
            if (person is not null)
            {
                var user = await userRepository.FindByPersonId(person.Id, cancellationToken);

                if (user is not null && user.IsActive)
                {
                    return MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.BadRequest)!;
                }
                var shahkarResponse = await CheckShahkar(user.Person.MobileNumber, user.Person.NationalCode, cancellationToken);
                if (!shahkarResponse.IsSuccess)
                {
                    return shahkarResponse;
                }
                user.Active();
                await userRepository.UpdateAsync(user, cancellationToken);
                await userRepository.SaveChangeAsync(cancellationToken);

                token = await GenerateActiveTokenAsync(request, cancellationToken);
                userData = await userRepository.GetByMobileNumberAsync(user.Person.MobileNumber, cancellationToken);
                return await MakeRegisterResponse(request, userData, token, cancellationToken);
            }

            var validateRegisterTokenStatus = jwtService.ValidateRegisterToken(request.RegisterToken);
            if (!validateRegisterTokenStatus.Item2 || validateRegisterTokenStatus.Item1.MobileNumber != request.MobileNumber)
            {
                var responseUnAuthorize = MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.UnAuthorized);
                responseUnAuthorize.Message = "عدم دسترسی";
                return responseUnAuthorize!;
            }

            var personModel = await userRepository.GetPersonByNationalCodeAsync(request.NationalCode, cancellationToken);
            if (personModel is not null && personModel.Id != 0)
            {
                var userExistResponse = await HandleUserExistWithAnotherNumber(request, personModel.MobileNumber, cancellationToken);
                if (userExistResponse.StatusCode != EResponseStatusCode.DoneSuccessfully)
                    return userExistResponse;
                var changedUser = await userRepository.GetByMobileNumberAsync(request.MobileNumber, cancellationToken);
                return await MakeRegisterResponse(request, changedUser, null, cancellationToken);
            }

            var userModel = await userRepository.GetByMobileNumberAsync(request.MobileNumber, cancellationToken);
            if (userModel is not null)
                return MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.YouAreAlreadyRegisteredAndHaveAnAccount)!;

            var response = await CheckInvitedCode(request, cancellationToken);
            if (response is not null)
                return response;

            var personTemp = InsertPersonTemp(request, cancellationToken);
            response = await CheckShahkar(request, personTemp, cancellationToken);
            if (response is not null)
                return response;

            await AddPersonAndApplicationUser(request, cancellationToken);
            token = await GenerateActiveTokenAsync(request, cancellationToken);
            userData = await userRepository.GetByMobileNumberAsync(personTemp.MobileNumber, cancellationToken);
            _ = await AddWallet(userData, token, _walletServiceUrl, _apiKeyInternal, cancellationToken);
            return await MakeRegisterResponse(request, userData, token, cancellationToken);
        }
        catch (Exception ex)
        {

            logger.LogInformation($"Register EXC ============>>>>>> {ex.Message}");
            return MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.InternalServerErrors)!;
        }
    }

    private async Task<BaseMessageResponse<RegisterUserDto>> MakeRegisterResponse(RegisterUserCommand request, UserModel user, string? token, CancellationToken cancellationToken)
    {
        var newUser = token is not null;
        token ??= await GenerateActiveTokenAsync(request, cancellationToken);
        CityInformationDTO city;
        if (user.CityId is 0 or null)
            city = new CityInformationDTO();
        else
            city = await registerLoginService.GetCityInformation(user.CityId.Value, cancellationToken);

        var resultTemp = await registerLoginService.MakeResponse(user, city, "", token, newUser, cancellationToken);
        var result = mapper.Map<RegisterUserDto>(resultTemp);
        return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, result)!;
    }

    private async Task<BaseMessageResponse<RegisterUserDto>> HandleUserExistWithAnotherNumber(RegisterUserCommand request, string oldMobileNumber, CancellationToken cancellationToken)
    {
        BaseMessageResponse<RegisterUserDto> response;
        var shahkarResponse = await CheckShahkar(request.MobileNumber, request.NationalCode, cancellationToken);
        if (!shahkarResponse.IsSuccess || CheckVersion(request.RequestInfo.Version))
            response = MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.YouAreAlreadyRegisteredAndHaveAnAccount)!;
        else
        {
            var changeRequest = new MakeChangeMobileNumberRequest(oldMobileNumber, request.MobileNumber, request.NationalCode);
            var changeResponse = await changeMobileNumberService.CheckAndChangeInactiveMobileNumberRequest(changeRequest, cancellationToken);
            if (changeResponse.IsSuccess)
                return MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.DoneSuccessfully)!;
            var token = await GenerateTokenAsync(request, oldMobileNumber, cancellationToken);
            var data = new RegisterUserDto(null, null, null, null, null, null, null, null, null, null, oldMobileNumber, token, null, null, null, null, null, null, null);
            response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.YouAreAlreadyRegisteredAndHaveAnAccountWithAnotherMobileNumber, data)!;
            response.IsSuccess = true;
        }

        return response!;
    }

    private static bool CheckVersion(string version)
    {
        return new Version(version).CompareTo(new Version("2.9.0")) < 0;
    }

    private async Task<BaseMessageResponse<RegisterUserDto>> CheckShahkar(string mobileNumber, string nationalCode, CancellationToken cancellationToken)
    {
        var shahkarServiceProvider = new ShahkarServiceProvider(configuration, redisCacheManager, shahkarOptions, httpClient);
        var checkIsMatched = await shahkarServiceProvider.ValidateIdentityByShahkar(new ShahkarRequestViewModel()
        {
            MobileNumber = mobileNumber,//_baseRequest.RequestInfo.MobileNumber,
            NationalId = nationalCode//personTemp.NationalCode,
        }, cancellationToken);
        if (checkIsMatched is not null)
        {
            if (checkIsMatched.Data.response != 200)
                return MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.ShahkarError)!;
        }
        else
            return MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.ShahkarServiceIsNotAvailable)!;
        return MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.DoneSuccessfully)!;
    }

    private async Task<string> GenerateActiveTokenAsync(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var activeToken = new ActiveTokenModel(request.MobileNumber, request.RequestInfo.DeviceInfo,
            request.RequestInfo.Version,
            request.RequestInfo.IP, DateTime.Now.ToTimeStampSeconds(), true);
        activeToken.GenerateToken();

        await activeTokenCommandRepository.DeactivateActiveTokenAsync(activeToken, cancellationToken);
        await activeTokenCommandRepository.InsertActiveTokenAsync(activeToken, cancellationToken);
        return activeToken.Token;
    }

    private async Task<string> GenerateTokenAsync(RegisterUserCommand request, string oldMobileNumber, CancellationToken cancellationToken)
    {
        var activeToken = new ActiveTokenModelBeforeLogin(request.MobileNumber,
            oldMobileNumber, request.RequestInfo.DeviceInfo,
            request.RequestInfo.Version, request.RequestInfo.IP, DateTime.Now.ToTimeStampSeconds(),
            DateTime.Now.AddSeconds(300), true);

        activeToken.GenerateToken();

        await activeTokenCommandRepository.DeactivateActiveTokenBeforeLoginAsync(activeToken, cancellationToken);
        await activeTokenCommandRepository.InsertActiveTokenBeforeLoginAsync(activeToken, cancellationToken);
        return activeToken.Token;
    }

    private async Task AddPersonAndApplicationUser(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var personModel = new PersonModel(0, "", "", request.NationalCode, request.MobileNumber);
        var person = await userRepository.AddPersonAsync(personModel, cancellationToken);
        var myInviteCode = InviteCode.Generate(request.NationalCode, Enums.InviteType.User);
        await userRepository.AddApplicationUserAsync(person.PersonId, request.InvitedCode, "", myInviteCode, cancellationToken);
    }

    private PersonTempModel InsertPersonTemp(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var personTemp = new PersonTempModel
        {
            LastSentActivationCodeDateTime = DateTime.Now,
            MobileNumber = request.MobileNumber,
            IsDeleted = false,
            RegisterDate = DateTime.Now,
            IsVerifiedActivationCode = false,
            IsVerifiedByShahkar = false,
            LastIpAddress = request.RequestInfo.IP,
            LastDeviceInfo = request.RequestInfo.DeviceInfo,
            InvitedCode = request.InvitedCode,
            NationalCode = request.NationalCode,
        };
        personTempRepository.InsertPersonTemp(personTemp, cancellationToken);
        return personTemp;
    }

    private async Task<BaseMessageResponse<RegisterUserDto>?> CheckInvitedCode(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.InvitedCode))
            return null;

        var invitedCodeStatus = await userRepository.ExistInvitedCodeAsync(request.InvitedCode, null, cancellationToken);
        if (invitedCodeStatus)
            return null;
        return MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.InvitedCodeError)!;
    }

    private async Task<BaseMessageResponse<RegisterUserDto>?> CheckShahkar(RegisterUserCommand request, PersonTempModel personTemp, CancellationToken cancellationToken)
    {
        var shahkarServiceProvider = new ShahkarServiceProvider(configuration, redisCacheManager, shahkarOptions, httpClient);

        logger.LogError("checkIsMatched nnn" + request.NationalCode);
        logger.LogError("checkIsMatched mmm" + request.MobileNumber);


        var checkIsMatched = await shahkarServiceProvider.ValidateIdentityByShahkar(new ShahkarRequestViewModel()
        {
            MobileNumber = request.MobileNumber,
            NationalId = request.NationalCode,
        }, cancellationToken);

        if (checkIsMatched != null)
        {
            if (checkIsMatched.Data.response == 200)
            {
                personTemp.IsVerifiedByShahkar = true;
                personTemp.VerifiedByShahkarDateTime = DateTime.Now;
                personTempRepository.UpdatePersonTemp(personTemp, cancellationToken);
                return null;
            }
            else
                return MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.ShahkarError)!;
        }
        else
            return MessageResponse.GenerateMessageResponse<RegisterUserDto>(EResponseStatusCode.InternalServerError)!;

    }

    private async Task<bool> AddWallet(UserModel user, string token, string url,
        string apikeyInternal, CancellationToken cancellationToken)
    {
        try
        {
            var requestUrl = $"{url}/v2/InternalWallet/AddWallet";
            var headers = new Dictionary<string, string>
                {
                    { "AppId", "6" },
                    { "DeviceTypeId", "9" },
                    { "AppLanguage" , "fa"},
                    { "DeviceId" , user.MobileNumber},
                    { "Authorization", $"Bearer {token}" },
                    { "ApiKeyInternal", apikeyInternal}

                };
            var model = new AddUserWalletModel
            {
                MobileNumber = user.MobileNumber,
                UserId = user.Id,
                WalletTypeId = 1
            };
            var response = await requestHelper.PostAsync(requestUrl, model, headers);
            var status = JsonSerializer.Deserialize<BaseMessageResponse<bool>>(response, JsonHelper.JsonCamelCaseOption());
            return status!.Data;
        }
        catch (Exception exp)
        {
            logger.LogError(exp.StackTrace);
            return false;
        }
    }
}