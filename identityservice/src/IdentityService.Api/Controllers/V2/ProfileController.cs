using IdentityService.Services.ViewModels.Request;
using IdentityService.Services.ViewModels.Response;
using IdentityService.Services.ViewModels.V2.AddAvatar.Request;
using IdentityService.Services.ViewModels.V2.Profile.Response;
using IdentityService.Services.ViewModels.V2.RemoveAvatar.Request;
using IdentityService.Services.ViewModels.V2.UpdateProfile.Request;
using OmidBank.Common.InternalThirdParty.ShahkarService.Request;
using OmidBank.Common.InternalThirdParty.ShahkarService;
using System.Net.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.RegularExpressions;
using OmidBank.Common.Cache;

using static OmidBank.Common.InternalThirdParty.ShahkarService.ShahkarServiceProvider;

namespace IdentityService.Api.Controllers.V2
{
    [ApiVersion("2")]
    public class ProfileController : BaseIdentityApplicationApi
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestHelper _requestHelper;
        private readonly ICityService _cityService;
        private readonly IWalletService _walletService;
        private readonly IActiveTokenRepository _activeTokenRepository;
        private readonly IAvatarImageService _avatarImageService;
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProfileController> _logger;
        private readonly string _fileServerUrl;
        private readonly string _extraDataServiceUrl;
        private readonly string _baseOmidTechUrl;
        private readonly string _apiKeyOmidTech;
        private readonly string _walletServiceUrl;
        private readonly string _logServiceUrl;
        private readonly string _avatarPathStorage;
        private readonly bool _isDevelopment;
        private readonly string _testFileServiceUrl;
        private readonly string _apiKeyInternal;
        private readonly string _avatarFileServerUrl;
        private readonly string _cdnBaseUrl;
        private readonly IOptions<ShahkarProviderOptions> _shahkarOptions;



        public ProfileController(IUserRepository userRepository,
                                 IConfiguration configuration,
                                 IRequestHelper requestHelper,
                                 ILogger<ProfileController> logger,
                                 ICityService cityService,
                                 IActiveTokenRepository activeTokenRepository,
                                 IAvatarImageService avatarImageService,
                                 IOptions<ShahkarProviderOptions> shahkarOptions,
                                 IRedisCacheManager redisCacheManager,
                                 HttpClient httpClient
            )
        {
            _userRepository = userRepository;
            _fileServerUrl = configuration["FileServiceUrl"];
            _testFileServiceUrl = configuration["TestFileServiceUrl"];
            _extraDataServiceUrl = configuration["ExtraDataServiceUrl"];
            _requestHelper = requestHelper;
            _logger = logger;
            _cityService = cityService;
            _baseOmidTechUrl = configuration["BaseOmidTechUrl"];
            _apiKeyOmidTech = configuration["ApiKeyOmidTech"];
            _walletServiceUrl = configuration["WalletServiceUrl"];
            _logServiceUrl = configuration["LogServiceUrl"];
            _activeTokenRepository = activeTokenRepository;
            _avatarPathStorage = configuration["AvatarPathStorage"];
            _avatarImageService = avatarImageService;
            _isDevelopment = bool.Parse(configuration["IsDevelopment"]);
            _apiKeyInternal = configuration["ApiKeyInternal"];
            _avatarFileServerUrl = configuration["AvatarFileServerUrl"];
            _shahkarOptions = shahkarOptions;
            _cdnBaseUrl = configuration["CdnBaseUrl"];
            _redisCacheManager = redisCacheManager;
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<ProfileDetailResponse>(EResponseStatusCode.DoneSuccessfully);

            var userProfile = await _userRepository.GetProfileAsync(_baseRequest.RequestInfo.MobileNumber, cancellationToken);
            if (userProfile is null)
                return BaseResult(MessageResponse.GenerateMessageResponse<ProfileDetailResponse>(EResponseStatusCode.ItemIsNotFound));

            userProfile.CdnUrl = userProfile.AvatarName != null ? _avatarFileServerUrl : null;

            //if (_baseRequest.RequestInfo.AppId == Enums.AppEnum.MobileApp)
            //    userProfile.CdnUrl = (double.Parse(_baseRequest.RequestInfo.Version) > 2.5 && userProfile.AvatarName != null) ?
            //        _avatarFileServerUrl
            //        : userProfile.AvatarName != null ? $"{_fileServerUrl}/{userProfile.AvatarName}"
            //        : null;
            //else
            //    userProfile.CdnUrl = userProfile.AvatarName != null ? (_baseRequest.RequestInfo.AppId != Enums.AppEnum.MobileApp) ?
            //        _avatarFileServerUrl
            //        : $"{_fileServerUrl}/{userProfile.AvatarName}"
            //        : null;

            userProfile.IsActiveProfile = userProfile.NationalCode != null ? true : false;
            var city = new CityInformationDTO();
            if (userProfile != null && userProfile.CityId != null)
                city = await _cityService.GetCityById(userProfile.CityId, cancellationToken);
            //
            // userProfile.CityTitle = city.Title;
            // userProfile.CityEnTitle = city.EnTitle;

            if (string.IsNullOrEmpty(userProfile.MyInviteCode))
            {
                //generate invited code
                var myInviteCode = InviteCode.Generate(userProfile.NationalCode, Enums.InviteType.User);
                userProfile.MyInviteCode = myInviteCode;
                await _userRepository.UpdateMyInviteCodeAsync(userProfile.Id, myInviteCode, cancellationToken);
                await _userRepository.SaveChangeAsync(cancellationToken);
            }

            response.Data = new()
            {
                AvatarUrl = userProfile.CdnUrl,
                CityEnTitle = city.CityEnTitle,
                CityId = userProfile.CityId,
                CityTitle = city.CityTitle,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                NationalCode = userProfile.NationalCode,
                InvitedCode = userProfile.InvitedCode,
                MyInviteCode = userProfile.MyInviteCode,
                ProvinceId = city.ProvinceId,
                ProvinceTitle = city.ProvinceTitle,
                ProvinceEnTitle = city.ProvinceEnTitle,
            };

            return BaseResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateProfileRequest profile, CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully);

            if (_baseRequest.RequestInfo.MobileNumber != profile.MobileNumber)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.WrongParameters));
            var user = await _userRepository.GetByMobileNumberAsync(profile.MobileNumber, cancellationToken);
            if (user == null)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.ItemIsNotFound));

            Regex FullNameRegex123 = new(@"^(?=.{1,120}$)\p{L}+(?:\s\p{L}+)*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            var name = profile.FirstName.Trim();
            var fullName = profile.LastName.Trim();
            var statusName = FullNameRegex123.IsMatch(name);
            var statusFullName = FullNameRegex123.IsMatch(fullName);

            if (!statusName || !statusFullName)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.WrongParameters));
            if (!_isDevelopment)
            {
                ShahkarServiceProvider shahkarServiceProvider = new ShahkarServiceProvider(_configuration, _redisCacheManager, _shahkarOptions, _httpClient);
                var checkIsMached = await shahkarServiceProvider.ValidateIdentityByShahkar(new ShahkarRequestViewModel()
                {
                    MobileNumber = profile.MobileNumber,
                    NationalId = profile.NationalCode,
                }, cancellationToken);
                if (checkIsMached != null)
                {
                    if (checkIsMached.Data.response != 200)
                        return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.ShahkarError));
                }
                else
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.CallErroProvider));
            }

            if (string.IsNullOrEmpty(user.InvitedCode) && !string.IsNullOrEmpty(profile.InviteCode))
            {
                var isExistInvitedCode = await _userRepository.ExistInvitedCodeAsync(profile.InviteCode, user.PersonId, cancellationToken);
                if (isExistInvitedCode)
                    await _userRepository.UpdateInvitedCodeAsync(user.PersonId, profile.InviteCode, cancellationToken);
                else
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.InvitedCodeError));
            }

            var result = await _userRepository.UpdateProfileAsync(new()
            {
                PersonId = user.PersonId,
                NationalCode = profile.NationalCode,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                MobileNumber = profile.MobileNumber,
                Address = profile.Address,
                CityId = profile.CityId,
                Gender = profile.Gender,
                Birthday = profile.Birthday,
                MyInviteCode = user.MyInviteCode
            }, cancellationToken);

            if (result > 0)
            {
                if (string.IsNullOrEmpty(user.NationalCode) && !string.IsNullOrEmpty(profile.NationalCode))
                    await _walletService.AddWallet(user, _baseRequest.RequestInfo.Token, _walletServiceUrl, _apiKeyInternal, cancellationToken);

            }
            else
                response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.ErrorOccurred);
            return BaseResult(response);
        }

        [HttpPost]
        //[RequestFormLimits]
        public async Task<IActionResult> AddAvatar([FromBody] AddAvatarRequest avatarModel, CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully);
            var user = await _userRepository.GetProfileAsync(avatarModel.MobileNumber, cancellationToken);
            if (user == null)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.ItemIsNotFound));

            if (!string.Equals(_baseRequest?.RequestInfo?.MobileNumber, avatarModel.MobileNumber, StringComparison.Ordinal))
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.WrongParameters));


            var fileName = $"{CommonHelper.GenerateLongRandomNumber()}.jpg";
            var status = false;
            if (_isDevelopment)
            {
                status = await UpdateFile(avatarModel.ImageFile, fileName, user.AvatarName);
            }
            else
                status = _avatarImageService.UpdateAvatarImage(avatarModel.ImageFile, fileName, user.AvatarName, _avatarPathStorage, cancellationToken);

            if (status)
                status = await _userRepository.UpdateAvatarAsync(null, avatarModel.MobileNumber, fileName, cancellationToken);

            if (!status)
                response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.ErrorOccurred);


            return BaseResult(response);
        }

        [HttpPost]
        public async Task<object> RemoveAvatar([FromBody] RemoveAvatarRequest avatarModel, CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully);

            var user = await _userRepository.GetProfileAsync(avatarModel.MobileNumber, cancellationToken);
            if (user == null)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.ItemIsNotFound));

            if (!string.Equals(_baseRequest?.RequestInfo?.MobileNumber, avatarModel.MobileNumber, StringComparison.Ordinal))
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.WrongParameters));

            if (!string.IsNullOrEmpty(user.AvatarName))
            {
                _avatarImageService.DeleteAvatarFile(user.AvatarName, _avatarPathStorage, cancellationToken);
                var status = await _userRepository.UpdateAvatarAsync(null, avatarModel.MobileNumber, null, cancellationToken);
                if (!status)
                    response = MessageResponse.GenerateMessageResponse<object>(EResponseStatusCode.ErrorOccurred);
            }
            else
                response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.ItemIsNotFound);


            return BaseResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> SetInviteCode([FromBody] UpdateInvitedCodeRequest request, CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully);


            var validateUpdateInvitedCodeRequest = await ValidateInviteCodeRequest(_baseRequest.RequestInfo.MobileNumber, request.InvitedCode, cancellationToken);
            if (!validateUpdateInvitedCodeRequest.IsSuccess)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.WrongParameters));

            var person = await _userRepository.GetPersonByMobileNumberAsync(_baseRequest.RequestInfo.MobileNumber, cancellationToken);
            if (!string.IsNullOrEmpty(request.InvitedCode))
            {
                var invitedCodeStatus = await _userRepository.ExistInvitedCodeAsync(request.InvitedCode, null, cancellationToken);
                if (!invitedCodeStatus)
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.InvitedCodeError));

                var status = await _userRepository.UpdateInvitedCodeAsync(person.Id, request.InvitedCode, cancellationToken);
                if (status)
                {
                    response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully, true);
                    return BaseResult(response);
                }
                else
                {
                    response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.WrongParameters, false);
                    return BaseResult(response);
                }

            }
            else
                return BaseResult(MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.WrongParameters, false));

        }

        [HttpGet]
        [SwaggerOperation(Summary = "وب سرویس دعوت شدگان من")]
        public async Task<IActionResult> GetMyInvited(CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<object>(EResponseStatusCode.DoneSuccessfully);
            var Items = new List<GetMyInvitedResponse>();
            var totalScore = 0;

            var person = await _userRepository.GetPersonByMobileNumberAsync(_baseRequest.RequestInfo.MobileNumber, cancellationToken);
            if (person == null)
            {
                response.Data = new { totalScore, Items };
                return BaseResult(response);
            }

            var user = await _userRepository.FindByPersonId(person.Id, cancellationToken);
            if (user == null)
            {
                response.Data = new { totalScore, Items };
                return BaseResult(response);
            }

            var invitedList = await _userRepository.GetMyInvitedCode(user.MyInviteCode, cancellationToken);
            foreach (var item in invitedList)
            {
                var personInvited = await _userRepository.GetPersonByIdAsync(item.PersonId, cancellationToken);
                if (personInvited != null)
                {
                    Items.Add(new GetMyInvitedResponse
                    {
                        MobileNumber = personInvited.MobileNumber.Substring(0, 2) + "****" + personInvited.MobileNumber.Substring(7, 4),
                        CreationTime = ((DateTimeOffset)personInvited.CreationTime).ToUnixTimeSeconds(),
                        Reason = "ثبت نام امید بانک",
                    });
                }
            }
            response.Data = new { totalScore, Items };
            return BaseResult(response);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "وب سرویس جزییات دعوت")]
        public async Task<IActionResult> GetDetailInvite(CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<DetailInviteResponse>(EResponseStatusCode.DoneSuccessfully);
            var person = await _userRepository.GetPersonByMobileNumberAsync(_baseRequest.RequestInfo.MobileNumber, cancellationToken);
            if (person == null)
            {
                return BaseResult(response);
            }

            var user = await _userRepository.FindByPersonId(person.Id, cancellationToken);
            if (user == null)
            {
                return BaseResult(response);
            }

            if (string.IsNullOrEmpty(user.MyInviteCode))
            {
                //generate invited code
                var myInviteCode = InviteCode.Generate(user.Person.NationalCode, Enums.InviteType.User);
                user.MyInviteCode = myInviteCode;
                await _userRepository.UpdateMyInviteCodeAsync(user.Id, myInviteCode, cancellationToken);
                await _userRepository.SaveChangeAsync(cancellationToken);
            }

            var phoneMasked = "\u202D" + person.MobileNumber.Substring(0, 2) +
                              "****" +
                              person.MobileNumber.Substring(7, 4);

            var description = _configuration["Invite:FirstDescription"] + phoneMasked +
               _configuration["Invite:SecondDescription"] + user.MyInviteCode;

            response.Data = new DetailInviteResponse
            {
                MyInviteCode = user.MyInviteCode,
                InviteDescription = description,
                InviteBannerUrl = _cdnBaseUrl + "/advertisement/invite.png"
            };

            return BaseResult(response);
        }

        [HttpPost]
        [AnonymousAttribute]
        public async Task<IActionResult> CheckInvitedCodeAsync([FromBody] UpdateInvitedCodeRequest request, CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<CheckInvitedCodeResponse>(EResponseStatusCode.DoneSuccessfully);
            var invitedCodeStatus = await _userRepository.ExistInvitedCodeAsync(request.InvitedCode, null, cancellationToken);
            if (!invitedCodeStatus)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.InvitedCodeError));
            else
            {
                var userData = await _userRepository.GetPersonDataWithInvitedCode(request.InvitedCode, cancellationToken);
                var responseCheckInvitedCode = new CheckInvitedCodeResponse();
                responseCheckInvitedCode.InvitedCodeOwnerName = userData is not null ? userData.FirstName : "";
                responseCheckInvitedCode.InvitedCodeOwnerLastName = userData is not null ? userData.LastName : "";
                responseCheckInvitedCode.Status = invitedCodeStatus;

                response.Data = responseCheckInvitedCode;
                return BaseResult(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvatar(CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<GetAvatarResponse>(EResponseStatusCode.DoneSuccessfully);
            var userProfile = await _userRepository.GetProfileAsync(_baseRequest.RequestInfo.MobileNumber, cancellationToken);
            if (userProfile.AvatarName is null)
            {
                response.Data.HasAvatar = false;
                response.Data.AvatarPath = null;
                return BaseResult(response);
            }
            var path = $"{_avatarPathStorage}/{userProfile.AvatarName}";

            var ttt = System.IO.File.Exists(path);

            _logger.LogInformation("1234" + ttt.ToString());
            _logger.LogInformation("path" + path);
            if (!System.IO.File.Exists(path))
                return NotFound();
            return PhysicalFile(path, "image/jpeg");
        }

        private async Task<bool> UpdateFile(string ImageFile, string FileName, string AvatarName)
        {
            {
                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, $"{_testFileServiceUrl}/v1/Public");
                    request.Headers.Add("accept", "*/*");
                    //request.Headers.Add("X-API-Key", "{{token}}");
                    var content = new StringContent(JsonSerializer.Serialize(new
                    {
                        imageFile = ImageFile,
                        fileName = FileName,
                        avatarName = AvatarName
                    }), null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    return bool.Parse(result);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private async Task<BaseMessageResponse<bool>> ValidateInviteCodeRequest(string mobileNumber, string invitedCode, CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully);

            var user = await _userRepository.GetByMobileNumberAsync(mobileNumber, cancellationToken);
            if (user == null)
            {
                response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.WrongParameters, false);
                return response;
            }

            if (!string.IsNullOrEmpty(user.InvitedCode))
            {
                response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.WrongParameters, false);
                return response;
            }
            else if (user.MyInviteCode == invitedCode)
            {
                response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.WrongParameters, false);
                return response;
            }
            else
            {
                response = MessageResponse.GenerateMessageResponse<bool>(EResponseStatusCode.DoneSuccessfully, true);
                return response;
            }
        }
    }
}
