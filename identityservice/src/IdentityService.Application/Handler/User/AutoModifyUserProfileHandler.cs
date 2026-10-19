using IdentityService.Application.Handler.User.Shared;
using IdentityService.Domain.Entity.Profile.UserPorfile;
using IdentityService.Domain.IRepositories;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.InternalThreadParty.OmidTech;
using System.Text.RegularExpressions;
using OmidBank.Common.InternalThirdParty.SabtahvalService;
using OmidBank.Common.InternalThirdParty.Metaboom.Request;
using Microsoft.Extensions.Options;
using OmidBank.Common.Helper.Provider;
using OmidBank.Common.Cache;

namespace IdentityService.Application.Handler.User;

public class AutoModifyUserProfileHandler : IBaseHandler<AutoModifyUserProfileRequest, BaseMessageResponse<AutoModifyUserProfileResponse>>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserRepository _userRepository;
    private readonly string _defaultEnglishName;
    private readonly IConfiguration _configuration;
    private readonly IOptions<ProviderOptions> _sabtAhvalOptions;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly HttpClient _httpClient;




    public AutoModifyUserProfileHandler(IUserProfileRepository userProfileRepository, IConfiguration configuration,
        IUserRepository userRepository, IOptions<ProviderOptions> sabtAhvalOptions,IRedisCacheManager redisCacheManager,
        HttpClient httpClient)
    {
        _userProfileRepository = userProfileRepository;
        _configuration = configuration;
        _userRepository = userRepository;
        _defaultEnglishName = configuration["DefaultEnglishName"];
        _sabtAhvalOptions = sabtAhvalOptions;
        _redisCacheManager  = redisCacheManager;
        _httpClient = httpClient;
    }

    public async Task<BaseMessageResponse<AutoModifyUserProfileResponse>> Handle(AutoModifyUserProfileRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<AutoModifyUserProfileResponse>(EResponseStatusCode.DoneSuccessfully, new());
        var omidTechProvider = new OmidTechProvider(_configuration["BaseOmidTechUrl"], _configuration["ApiKeyOmidTech"], _httpClient);
        var findUserProfile = await _userProfileRepository.FindByUserId(request.CurrentUser.Id, cancellationToken);
        if (findUserProfile is not null && findUserProfile.InquiryLastDate is not null && (DateTime.Now - findUserProfile.InquiryLastDate.Value).TotalMinutes <= 360)
        {
            response.Data.RealFirstName = findUserProfile.RealFirstName;
            response.Data.RealLastName = findUserProfile.RealLastName;
            response.Data.FatherName = findUserProfile.FatherName;
            response.Data.DeathStatus = findUserProfile.DeathStatus;
            return response;
        }

        var user = await _userRepository.FindById(request.CurrentUser.Id, cancellationToken);

        if (user.Person.BirthDate is null)
            return MessageResponse.GenerateMessageResponse<AutoModifyUserProfileResponse>(EResponseStatusCode.BirthDateNotEnteredInSystem);

        var provider = new SabtahvalProvider(_configuration ,_sabtAhvalOptions, _redisCacheManager, _httpClient);

        var resultPersonalInfo = await provider.GetPersonalInfo(new SabtAhvalInquiryRequest
        {
            NationalCode = request.CurrentUser.NationalCode,
            BirthDate = user.Person.BirthDate.Value
        }, cancellationToken);


        if (resultPersonalInfo is null || !resultPersonalInfo.Success)
            return MessageResponse.GenerateMessageResponse<AutoModifyUserProfileResponse>(EResponseStatusCode.CallErrorOmidTech);

        if (findUserProfile is null)
        {
            findUserProfile = new UserProfileEntity()
            {
                UserId = request.CurrentUser.Id,
                RealFirstName = resultPersonalInfo.Data.FirstName,
                RealLastName = resultPersonalInfo.Data.LastName,
                FatherName = resultPersonalInfo.Data.FatherName,
                IdentityCardNumber = resultPersonalInfo.Data.CertNumber != null ?
                resultPersonalInfo.Data.CertNumber.ToString() : null,
                IdentityCardSeries = resultPersonalInfo.Data.IdentificationSeri,
                IdentityCardSerial = resultPersonalInfo.Data.IdentificationSerial,
                ResidentialStatusId = "0",
                BirthCertificateNo = resultPersonalInfo.Data.IdentificationSeri,
                BirthCertificateSerialNo = resultPersonalInfo.Data.IdentificationSerial,
                CustomerGroupId = "0",
                DeathStatus = (resultPersonalInfo.Data.DeathStatus == 0) ? false : true,
                SexId = resultPersonalInfo.Data.Gender == "1" ? "MALE" : "FEMALE", //46346 Men , 46347 Female
                SurnameId = "0",
                InquiryLastDate = DateTime.Now,
                RealFirstNameEn = !Regex.IsMatch(CommonHelper.PersianToFinglish(resultPersonalInfo.Data.FirstName),
                @"[^A-Za-z]") ? CommonHelper.PersianToFinglish(resultPersonalInfo.Data.FirstName) : _defaultEnglishName,
                RealLastNameEn = !Regex.IsMatch(CommonHelper.PersianToFinglish(resultPersonalInfo.Data.LastName),
                @"[^A-Za-z]") ? CommonHelper.PersianToFinglish(resultPersonalInfo.Data.LastName) : _defaultEnglishName,
                FatherNameEn = !Regex.IsMatch(CommonHelper.PersianToFinglish(resultPersonalInfo.Data.FatherName),
                @"[^A-Za-z]") ? CommonHelper.PersianToFinglish(resultPersonalInfo.Data.FatherName) : _defaultEnglishName,
            };
            await _userProfileRepository.AddAsync(findUserProfile, cancellationToken);
            await _userProfileRepository.SaveChangeAsync(cancellationToken);
        }
        else
        {
            findUserProfile.RealFirstName = resultPersonalInfo.Data.FirstName;
            findUserProfile.RealLastName = resultPersonalInfo.Data.LastName;
            findUserProfile.FatherName = resultPersonalInfo.Data.FatherName;
            findUserProfile.IdentityCardNumber = resultPersonalInfo.Data.CertNumber != null ? resultPersonalInfo.Data.CertNumber.ToString() : null;
            findUserProfile.IdentityCardSeries = resultPersonalInfo.Data.IdentificationSeri;
            findUserProfile.IdentityCardSerial = resultPersonalInfo.Data.IdentificationSerial;
            findUserProfile.ResidentialStatusId = "0";
            findUserProfile.BirthCertificateNo = resultPersonalInfo.Data.IdentificationSeri;
            findUserProfile.BirthCertificateSerialNo = resultPersonalInfo.Data.IdentificationSerial;
            findUserProfile.CustomerGroupId = "0";
            findUserProfile.DeathStatus = (resultPersonalInfo.Data.DeathStatus == 0) ? false : true;
            findUserProfile.SexId = resultPersonalInfo.Data.Gender == "1" ? "MALE" : "FEMALE"; //46346 Men , 46347 Female
            findUserProfile.SurnameId = "0";
            findUserProfile.InquiryLastDate = DateTime.Now;
            findUserProfile.RealFirstNameEn = !Regex.IsMatch(CommonHelper.PersianToFinglish(resultPersonalInfo.Data.FirstName), @"[^A-Za-z]") ?
                     CommonHelper.PersianToFinglish(resultPersonalInfo.Data.FirstName) : _defaultEnglishName;

            findUserProfile.RealLastNameEn = !Regex.IsMatch(CommonHelper.PersianToFinglish(resultPersonalInfo.Data.LastName), @"[^A-Za-z]") ?
                    CommonHelper.PersianToFinglish(resultPersonalInfo.Data.LastName) : _defaultEnglishName;

            findUserProfile.FatherNameEn = !Regex.IsMatch(CommonHelper.PersianToFinglish(resultPersonalInfo.Data.FatherName), @"[^A-Za-z]") ?
                 CommonHelper.PersianToFinglish(resultPersonalInfo.Data.FatherName) : _defaultEnglishName;

            await _userProfileRepository.UpdateAsync(findUserProfile, cancellationToken);
            await _userProfileRepository.SaveChangeAsync(cancellationToken);
        }
        response.Data.RealFirstName = findUserProfile.RealFirstName;
        response.Data.RealLastName = findUserProfile.RealLastName;
        response.Data.FatherName = findUserProfile.FatherName;
        response.Data.DeathStatus = findUserProfile.DeathStatus;
        return response;
    }
}


public record AutoModifyUserProfileRequest() : BaseCommand<BaseMessageResponse<AutoModifyUserProfileResponse>>;

public class AutoModifyUserProfileResponse : UserProfileResponseDto
{
    public string RealFirstName { get; set; }
    public string RealLastName { get; set; }
    public string FatherName { get; set; }
    public bool? DeathStatus { get; set; }
}