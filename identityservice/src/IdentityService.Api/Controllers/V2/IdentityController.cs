using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.UserManagement.UserManagmentBiometric;
using IdentityService.Services.IServices.AradPushNotificationAddUser;
using IdentityService.Services.ViewModels.Request;
using IdentityService.Services.ViewModels.Response;
using OmidBank.Common.Cache;
using OmidBank.Common.Helper.Provider;
using OmidBank.Common.InternalThirdParty.ShahkarService;
using OmidBank.Common.InternalThirdParty.ShahkarService.Request;
using OmidBank.Common.InternalThirdParty.SmsService.Request;
using OmidBank.Common.InternalThirdParty.WalletService;
using System.Net.Http;
using static OmidBank.Common.InternalThirdParty.ShahkarService.ShahkarServiceProvider;
namespace IdentityService.Api.Controllers.V2
{
    [ApiVersion("2")]
    public class IdentityController : BaseIdentityApplicationApi
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IActiveTokenRepository _activeTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPersonTempRepository _personTempRepository;
        private readonly string _walletServiceUrl;
        private readonly ISmsService _smsService;
        private readonly string _messageServiceUrl;
        private readonly string _apiKeyInternal;
        private readonly IRequestHelper _requestHelper;
        private readonly string _baseOmidTechUrl;
        private readonly string _apiKeyOmidTech;
        private readonly string _extraDataServiceUrl;
        private readonly IWalletService _walletService;
        private readonly string _fileServerUrl;
        private readonly IAradPushNotification _iAradPushNotification;
        private readonly IHttpContextAccessor _accessor;
        public readonly bool _isDevelopment;
        private readonly IUserManagementRepository _userManagementRepository;
        private readonly IUserManagementBiometricRepository _userManagementBiometricRepository;
        private readonly string _logServiceUrl;
        private readonly RequestHelperV2 _requestHelper2 = new();
        private readonly string _applicationVersionV2, _avatarFileServerUrl;
        private readonly bool _changePasswordWalletIsActive;
        private readonly double _timeoutAradServices;
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly IConfiguration _configuration;
        private readonly IOptions<ProviderOptions> _sabtAhvalOptions;
        private readonly IOptions<ShahkarProviderOptions> _shahkarOptions;
        private readonly ICityService _cityService;
        private readonly IOptions<SmsProviderOptions> _smsProviderOptions;
        private readonly HttpClient _httpClient;

        public IdentityController(ILogger<IdentityController> logger, IActiveTokenRepository activeTokenRepository,
            IUserRepository userRepository, IPersonTempRepository personTempRepository, ICityService cityService,
            IConfiguration configuration, IHttpContextAccessor accessor, ISmsService smsService,
            IRequestHelper requestHelper, IWalletService walletService,
            IAradPushNotification iAradPushNotification, IUserManagementRepository userManagementRepository,
            IRedisCacheManager redisCacheManager, IOptions<ProviderOptions> sabtAhvalOptions, IOptions<ShahkarProviderOptions> shahkarOptions
            , IOptions<SmsProviderOptions> smsProviderOptions, HttpClient httpClient
            )
        {
            _logger = logger;
            _activeTokenRepository = activeTokenRepository;
            _userRepository = userRepository;
            _personTempRepository = personTempRepository;
            _cityService = cityService;
            _accessor = accessor;
            _walletServiceUrl = configuration["WalletServiceUrl"];
            _smsService = smsService;
            _apiKeyInternal = configuration["ApiKeyInternal"];
            _requestHelper = requestHelper;
            _apiKeyOmidTech = configuration["ApiKeyOmidTech"];
            _extraDataServiceUrl = configuration["ExtraDataServiceUrl"];
            _walletService = walletService;
            _iAradPushNotification = iAradPushNotification;
            _fileServerUrl = configuration["FileServiceUrl"];
            _isDevelopment = bool.Parse(configuration["IsDevelopment"]);
            _messageServiceUrl = configuration["MessageServiceUrl"];
            _baseOmidTechUrl = configuration["BaseOmidTechUrl"];
            _userManagementRepository = userManagementRepository;
            _logServiceUrl = configuration["LogServiceUrl"];
            _applicationVersionV2 = configuration["ApplicationVersionV2"];
            _changePasswordWalletIsActive = bool.Parse(configuration["ChangePasswordWalletIsActive"]);
            _timeoutAradServices = double.Parse(configuration["TimeoutAradServices"]);
            _redisCacheManager = redisCacheManager;
            _configuration = configuration;
            _avatarFileServerUrl = configuration["AvatarFileServerUrl"];
            _sabtAhvalOptions = sabtAhvalOptions;
            _shahkarOptions = shahkarOptions;
            _smsProviderOptions = smsProviderOptions;
            _httpClient = httpClient;

        }

        [HttpGet("test")]
        [Anonymous]
        public async Task<object> HealthCheck(CancellationToken cancellationToken)
        {
            return new
            {
                Mess = "hello",
                ver = "v2.0",
                name = "identity service api"
            };
        }

        [HttpPost]
        [AnonymousAttribute]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request,
            CancellationToken cancellationToken)
        {
            string actionName = ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            _baseRequest.RequestInfo.RequestUrl = $"/v2/{controllerName}/{actionName}";

            double requestVersion = 0;
            if (!string.IsNullOrEmpty(_baseRequest.RequestInfo.Version))
                requestVersion = double.Parse(_baseRequest.RequestInfo.Version);

            if (requestVersion <= 2.4 && _baseRequest.RequestInfo.AppId == Enums.AppEnum.MobileApp)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.RegisterUpdateMessage));

            if (string.IsNullOrEmpty(request.MobileNumber) || string.IsNullOrEmpty(request.NationalCode))
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.MobileNumberAndNationalCodeIsRequired));

            var personId = await _userRepository.GetPersonIdNationalCodeAsync(request.NationalCode, cancellationToken);
            if (personId != null && personId != 0)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.YouAreAlreadyRegisteredAndHaveAnAccount));

            var user = await _userRepository.GetByMobileNumberAsync(request.MobileNumber, cancellationToken);
            if (user != null)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.YouAreAlreadyRegisteredAndHaveAnAccount));

            if (!string.IsNullOrEmpty(request.InvitedCode))
            {
                var invitedCodeStatus =
                    await _userRepository.ExistInvitedCodeAsync(request.InvitedCode, null, cancellationToken);
                if (!invitedCodeStatus)
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.InvitedCodeError));
            }

            var activationCode = new ActivationCodeModel(request.MobileNumber);
            activationCode.Type = Enums.ActivationCode.Register;
            if (_isDevelopment)
                activationCode.GenerateCode(true);
            else
                activationCode.GenerateCode(false);

            activationCode.SetTime(DateTime.Now);
            var personTemp = new PersonTempModel()
            {
                LastSentActivationCodeDateTime = DateTime.Now,
                ActivationCode = int.Parse(_isDevelopment ? "123456" : activationCode.Code),
                MobileNumber = request.MobileNumber,
                IsDeleted = false,
                RegisterDate = DateTime.Now,
                IsVerifiedActivationCode = false,
                IsVerifiedByShahkar = false,
                LastIpAddress = _baseRequest.RequestInfo.IP,
                LastDeviceInfo = _baseRequest.RequestInfo.DeviceInfo,
                InvitedCode = request.InvitedCode,
                NationalCode = request.NationalCode
            };
            _personTempRepository.InsertPersonTemp(personTemp, cancellationToken);

            var reqUpdateActivetoken = new ActivationCodeModel(request.MobileNumber);
            await _activeTokenRepository.UpdateActivationCodeAsync(reqUpdateActivetoken, cancellationToken);
            await _activeTokenRepository.AddActivationCodeAsync(activationCode, cancellationToken);
            if (_isDevelopment)
            {
                _baseRequest.RequestInfo.ServiceStatus = 1;
                _baseRequest.RequestInfo.ServiceStatus = 1;
                await AddLog(_baseRequest.RequestInfo, request, cancellationToken);
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully));
            }
            else
            {
                var sendSmsRequest = new SendActivitionCodeOmidTechViewModel()
                {
                    MobileNumber = request.MobileNumber,
                    Scheduler = null,
                    Code = activationCode.Code,
                    Url = _messageServiceUrl,
                    ApiKeyInternal = _apiKeyInternal
                };
                var activationSmsStatus =
                    await _smsService.SendActivationCodeFromThirdPartyOmidTech(sendSmsRequest, cancellationToken);

                if (activationSmsStatus)
                {
                    _baseRequest.RequestInfo.ServiceStatus = 1;
                    await AddLog(_baseRequest.RequestInfo, request, cancellationToken);
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully));
                }
                else
                {
                    await AddLog(_baseRequest.RequestInfo, request, cancellationToken);
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.InternalServerError));
                }
            }
        }

        [HttpPost]
        [AnonymousAttribute]
        public async Task<IActionResult> VerifyMobileNumber([FromBody] VerifyMobileNumberRegisterRequest request,
            CancellationToken cancellationToken)
        {
            string actionName = ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            _baseRequest.RequestInfo.RequestUrl = $"/v2/{controllerName}/{actionName}";

            var personTemp =
                _personTempRepository.GetLastPersonTempByMobileNumber(_baseRequest.RequestInfo.MobileNumber,
                    cancellationToken);
            if (personTemp == null)
                return BaseResult(MessageResponse.GenerateMessageResponse<object>(EResponseStatusCode.VerifyMobileNumberError));

            var user = await _userRepository.GetByMobileNumberAsync(_baseRequest.RequestInfo.MobileNumber,
                cancellationToken);
            if (user != null)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.VerifyMobileNumberError));

            var activationCode = new ActivationCodeModel(_baseRequest.RequestInfo.MobileNumber, request.OtpCode);
            activationCode.SetTime(DateTime.Now);
            activationCode.Type = Enums.ActivationCode.Register;

            var status = await _activeTokenRepository.VerifyActivationCodeAsync(activationCode, cancellationToken);
            await _activeTokenRepository.UpdateActivationCodeAsync(activationCode, cancellationToken);

            if (!status)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.VerifyMobileNumberError));

            if (!_isDevelopment)
            {
                ShahkarServiceProvider shahkarServiceProvider = new ShahkarServiceProvider(_configuration, _redisCacheManager, _shahkarOptions, _httpClient);
                var checkIsMached = await shahkarServiceProvider.ValidateIdentityByShahkar(new ShahkarRequestViewModel()
                {
                    MobileNumber = _baseRequest.RequestInfo.MobileNumber,
                    NationalId = personTemp.NationalCode,
                }, cancellationToken);
                if (checkIsMached != null)
                {
                    if (checkIsMached.Data.response == 200)
                    {
                        personTemp.IsVerifiedByShahkar = true;
                        personTemp.VerifiedByShahkarDateTime = DateTime.Now;
                        _personTempRepository.UpdatePersonTemp(personTemp, cancellationToken);
                        if (request.OtpCode != personTemp.ActivationCode.ToString())
                            return BaseResult(
                                MessageResponse.GenerateMessageResponse(EResponseStatusCode.VerifyMobileNumberError));
                    }
                    else
                        return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.ShahkarError));
                }
                else
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.InternalServerError));
            }

            var person = await _userRepository.AddPersonAsync(new PersonModel(0,
                "",
                "",
                personTemp.NationalCode,
                personTemp.MobileNumber
            ), cancellationToken);
            var myInviteCode = InviteCode.Generate(personTemp.NationalCode, Enums.InviteType.User);
            await _userRepository.AddApplicationUserAsync(person.PersonId, personTemp.InvitedCode, "", myInviteCode,
                cancellationToken);

            var currentPerson = new UserModel(personTemp.MobileNumber);


            if (!string.IsNullOrEmpty(personTemp.InvitedCode))
                await _userRepository.UpdateInvitedCodeAsync(currentPerson.PersonId, personTemp.InvitedCode,
                    cancellationToken);

            var activeToken = new ActiveTokenModel(personTemp.MobileNumber, _baseRequest.RequestInfo.DeviceInfo,
                _baseRequest.RequestInfo.Version,
                _baseRequest.RequestInfo.IP, DateTime.Now.ToTimeStampSeconds(), true);
            activeToken.GenerateToken();

            await _activeTokenRepository.GenerateTokenAsync(activeToken, cancellationToken);
            currentPerson.SetToken(activeToken.Token);

            var userData = await _userRepository.GetByMobileNumberAsync(personTemp.MobileNumber, cancellationToken);
            var addWalletResult =
                await _walletService.AddWallet(userData, currentPerson.Token, _walletServiceUrl, _apiKeyInternal, cancellationToken);

            _baseRequest.RequestInfo.ServiceStatus = 1;
            await AddLog(_baseRequest.RequestInfo, request, cancellationToken);
            return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully));
        }

        [HttpPost]
        [AnonymousAttribute]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            string actionName = ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            _baseRequest.RequestInfo.RequestUrl = $"/v2/{controllerName}/{actionName}";

            double requestVersion = 0;
            if (!string.IsNullOrEmpty(_baseRequest.RequestInfo.Version))
                requestVersion = double.Parse(_baseRequest.RequestInfo.Version);

            if (requestVersion <= 2.4 && _baseRequest.RequestInfo.AppId == Enums.AppEnum.MobileApp)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.LoginUpdateMessage));


            var user = await _userRepository.GetByMobileNumberAsync(request.MobileNumber, cancellationToken);
            if (user == null || !user.IsActive)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.RegisterFirstToLogIn));

            var activationCode = new ActivationCodeModel(request.MobileNumber);
            activationCode.Type = Enums.ActivationCode.Login;

            if (_isDevelopment)
                activationCode.GenerateCode(true);
            else
                activationCode.GenerateCode(false);

            activationCode.SetTime(DateTime.Now);
            var reqUpdateActivetoken = new ActivationCodeModel(request.MobileNumber);
            await _activeTokenRepository.UpdateActivationCodeAsync(reqUpdateActivetoken, cancellationToken);
            await _activeTokenRepository.AddActivationCodeAsync(activationCode, cancellationToken);

            if (_isDevelopment)
            {
                _baseRequest.RequestInfo.ServiceStatus = 1;
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully));
            }
            else
            {
                var sendSmsRequest = new SendActivitionCodeOmidTechViewModel()
                {
                    MobileNumber = request.MobileNumber,
                    Scheduler = null,
                    Code = activationCode.Code,
                    Url = _messageServiceUrl,
                    ApiKeyInternal = _apiKeyInternal
                };
                var activationSmsStatus =
                    await _smsService.SendActivationCodeFromThirdPartyOmidTech(sendSmsRequest, cancellationToken);

                if (activationSmsStatus)
                {
                    _baseRequest.RequestInfo.ServiceStatus = 1;
                    AddLog(_baseRequest.RequestInfo, request, cancellationToken);
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully));
                }
                else
                {
                    AddLog(_baseRequest.RequestInfo, request, cancellationToken);
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.InternalServerError));
                }
            }
        }

        [HttpPost]
        [AnonymousAttribute]
        public async Task<IActionResult> VerifyOtpCode([FromBody] VerifyCodeLogin request, CancellationToken cancellationToken)
        {
            string actionName = ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            _baseRequest.RequestInfo.RequestUrl = $"/v2/{controllerName}/{actionName}";
            var response = MessageResponse.GenerateMessageResponse<VerifyCodeLoginResponse>(EResponseStatusCode.DoneSuccessfully);

            var user = await _userRepository.GetByMobileNumberAsync(_baseRequest.RequestInfo.MobileNumber,
                cancellationToken);
            if (user == null)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.RegisterFirstToLogIn));

            var activationCode = new ActivationCodeModel(_baseRequest.RequestInfo.MobileNumber, request.OtpCode);
            activationCode.SetTime(DateTime.Now);

            var status = await _activeTokenRepository.VerifyActivationCodeAsync(activationCode, cancellationToken);
            await _activeTokenRepository.UpdateActivationCodeAsync(activationCode, cancellationToken);

            if (!status)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.WrongParameters));

            var activeToken = new ActiveTokenModel(_baseRequest.RequestInfo.MobileNumber,
                _baseRequest.RequestInfo.DeviceInfo,
                _baseRequest.RequestInfo.Version, _baseRequest.RequestInfo.IP, DateTime.Now.ToTimeStampSeconds(), true);

            activeToken.GenerateToken();

            await _activeTokenRepository.GenerateTokenAsync(activeToken, cancellationToken);
            user.SetToken(activeToken.Token);

            var city = new CityInformationDTO();
            if (user != null && user.CityId != null)
                city = await _cityService.GetCityById(user.CityId, cancellationToken);
            // else
            // {
            //     city.Id = 0;
            //     city.ParentId = null;
            //     city.Title = null;
            //     city.EnTitle = null;
            // }

            var applicationVersion = !string.IsNullOrEmpty(_baseRequest.RequestInfo.Version) ? _baseRequest.RequestInfo.Version : _applicationVersionV2;
            var personTempInfo = await _personTempRepository.GetPersonTempInfo(_baseRequest.RequestInfo.MobileNumber, cancellationToken);
            if (personTempInfo is not null)
                await _personTempRepository.UpdateApplicationVersion(personTempInfo.Id, applicationVersion, cancellationToken);

            var avatarUrl = string.Empty;
            if (_baseRequest.RequestInfo.AppId == Enums.AppEnum.MobileApp)
                avatarUrl = (double.Parse(_baseRequest.RequestInfo.Version) > 2.5 && user.AvatarName != null) ?
                    _avatarFileServerUrl
                    : user.AvatarName != null ? $"{_fileServerUrl}/{user.AvatarName}"
                    : null;
            else
                avatarUrl = user.AvatarName != null ? (_baseRequest.RequestInfo.AppId != Enums.AppEnum.MobileApp) ?
                    _avatarFileServerUrl
                    : $"{_fileServerUrl}/{user.AvatarName}"
                    : null;

            if (string.IsNullOrEmpty(user.MyInviteCode))
            {
                //generate invited code
                var myInviteCode = InviteCode.Generate(user.NationalCode, Enums.InviteType.User);
                user.MyInviteCode = myInviteCode;
                await _userRepository.UpdateMyInviteCodeAsync(user.PersonId, myInviteCode, cancellationToken);
                await _userRepository.SaveChangeAsync(cancellationToken);
            }

            var result = new VerifyCodeLoginResponse()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                CityId = user.CityId,
                CityTitle = city.CityTitle,
                CityEnTitle = city.CityEnTitle,
                ProvinceId = user.CityId,
                ProvinceTitle = city.CityTitle,
                ProvinceEnTitle = city.CityEnTitle,
                AvatarUrl = avatarUrl,
                MobileNumber = user.MobileNumber,
                Token = user.Token,
                InvitedCode = user.InvitedCode,
                NationalCode = user.NationalCode,
                MyInvitedCode = user.MyInviteCode
            };
            await _userRepository.UpdateFierbaseToken(user.MobileNumber, request.FirebaseKey);
            //if (!string.IsNullOrWhiteSpace(request.FirebaseKey) && !_isDevelopment)
            //{
            //    try
            //    {
            //        var clientInfo = CommonHelper.GetRequestHeaders(HttpContext);

            //        _logger.LogInformation("Arad GetUserDeviceList start:" + DateTime.Now.ToString());

            //        var device = await _iAradPushNotification.GetUserDeviceList(user.MobileNumber, _timeoutAradServices);

            //        _logger.LogInformation("end GetUserDeviceList start:" + DateTime.Now.ToString());


            //        if (device is { Data: not null } && device.Data.Any())
            //        {
            //            _logger.LogInformation("Arad RemoveUser start:" + DateTime.Now.ToString());

            //            await _iAradPushNotification.RemoveUser(user.MobileNumber,
            //                device.Data.FirstOrDefault().FcmToken, _timeoutAradServices);

            //            _logger.LogInformation("Arad RemoveUser end:" + DateTime.Now.ToString());

            //        }
            //        _logger.LogInformation("Arad AddUser start:" + DateTime.Now.ToString());

            //        var resultAddUser = await _iAradPushNotification.AddUser(new AradAddUserViewModel
            //        {
            //            ApplicationVersion = clientInfo.Version,
            //            BrowserName = "",
            //            DeviceName = clientInfo.DeviceInfo,
            //            DeviceVersion = "",
            //            FcmToken = request.FirebaseKey,
            //            OsName = clientInfo.UserAgent,
            //            Type = (int)clientInfo.DeviceTypeId,
            //            Mobile = user.MobileNumber
            //        }, _timeoutAradServices);
            //        _logger.LogInformation("Arad AddUser end:" + DateTime.Now.ToString());

            //    }
            //    catch (Exception exp)
            //    {
            //        _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
            //    }
            //}

            _baseRequest.RequestInfo.ServiceStatus = 1;
            AddLog(_baseRequest.RequestInfo, request, cancellationToken);
            if (_changePasswordWalletIsActive)
            {
                WalletServiceProvider walletServiceProvider = new WalletServiceProvider(_walletServiceUrl, _apiKeyInternal);
                var findWalletInfo = await walletServiceProvider.WalletInfoAsync(new OmidBank.Common.InternalThirdParty.WalletService.Request.WalletInfoRequest()
                {
                    UserId = user.Id,
                    WalletTypeId = 1,
                }, cancellationToken);
                if (findWalletInfo.IsSuccess && findWalletInfo.Data is not null)
                {
                    result.ChangePasswordWallet = findWalletInfo.Data.LastChangePassword =***;
                    result.OmidBankWalletAccountNumber = findWalletInfo.Data.AccountNumber;
                }
            }
            return BaseResult(MessageResponse.GenerateMessageResponse<VerifyCodeLoginResponse>(EResponseStatusCode.DoneSuccessfully, result));
        }

        [HttpPost]
        [AnonymousAttribute]
        public async Task<IActionResult> ResendOtpCode([FromBody] ResendOtpCodeRequest request,
            CancellationToken cancellationToken)
        {
            if (request.OtpCodeType == Enums.ActivationCode.Login)
            {
                var user = await _userRepository.GetByMobileNumberAsync(_baseRequest.RequestInfo.MobileNumber,
                    cancellationToken);
                if (user == null)
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.WrongParameters));
            }

            if (_baseRequest.RequestInfo.MobileNumber == null)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.InternalServerError));

            var activationCode = new ActivationCodeModel(_baseRequest.RequestInfo.MobileNumber);
            activationCode.Type = request.OtpCodeType;

            if (_isDevelopment)
                activationCode.GenerateCode(true);
            else
                activationCode.GenerateCode(false);

            activationCode.SetTime(DateTime.Now);
            var reqUpdateActivetoken = new ActivationCodeModel(_baseRequest.RequestInfo.MobileNumber);
            await _activeTokenRepository.UpdateActivationCodeAsync(reqUpdateActivetoken, cancellationToken);
            await _activeTokenRepository.AddActivationCodeAsync(activationCode, cancellationToken);
            if (_isDevelopment)
            {
                _baseRequest.RequestInfo.ServiceStatus = 1;
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully));
            }
            else
            {
                var sendSmsRequest = new SendActivitionCodeOmidTechViewModel()
                {
                    MobileNumber = _baseRequest.RequestInfo.MobileNumber,
                    Scheduler = null,
                    Code = activationCode.Code,
                    Url = _messageServiceUrl,
                    ApiKeyInternal = _apiKeyInternal
                };
                var activationSmsStatus =
                    await _smsService.SendActivationCodeFromThirdPartyOmidTech(sendSmsRequest, cancellationToken);

                if (activationSmsStatus)
                {
                    _baseRequest.RequestInfo.ServiceStatus = 1;
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully));
                }
                else
                    return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.InternalServerError));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignOut([FromBody] SignOutRequest request, CancellationToken cancellationToken)
        {
            if (request.MobileNumber != _baseRequest.RequestInfo.MobileNumber)
                return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.WrongParameters));

            await _activeTokenRepository.DeActiveTokenAsync(_baseRequest.RequestInfo.MobileNumber,
                _baseRequest.RequestInfo.Token, cancellationToken);
            UserModel currentPerson =
                await _userRepository.GetByMobileNumberAsync(_baseRequest.RequestInfo.MobileNumber, cancellationToken);
            await _userRepository.InquiryFireBaseToken(currentPerson.Id, cancellationToken);

            _redisCacheManager.Remove(string.Format(CacheKey.TosanUserSessionId, _baseRequest.RequestInfo.Token));
            return BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully));


            //var userManagementData =
            //    await _userManagementRepository.GetMobileNumberByUserName(_baseRequest.RequestInfo.MobileNumber,
            //        cancellationToken);
            //if (userManagementData != null && userManagementData.Id != null)
            //{
            //    await _userManagementRepository.LogoutUM(_baseRequest.RequestInfo.MobileNumber, cancellationToken);
            //    await _userManagementBiometricRepository.DeActiveBiometricToken(userManagementData.Id,
            //        cancellationToken);
            //}
        }

        private async Task AddLog(RequestInfo userInfo, object body, CancellationToken cancellationToken)
        {
            var requestUrl = $"{_logServiceUrl}/v2/InternalLog/Add";
            var jsonBody = string.Empty;
            var responseModel = string.Empty;
            if (body != null)
                jsonBody = JsonSerializer.Serialize(body, JsonHelper.JsonCamelCaseOption());

            if (userInfo.ResponseModel != null)
                responseModel = JsonSerializer.Serialize(userInfo.ResponseModel, JsonHelper.JsonCamelCaseOption());

            var model = new
            {
                Url = userInfo.RequestUrl,
                IP = userInfo.IP,
                Token = userInfo.Token,
                Version = userInfo.Version,
                AppId = userInfo.AppId,
                DeviceTypeId = userInfo.DeviceTypeId,
                DeviceInfo = userInfo.DeviceInfo,
                UserAgent = userInfo.UserAgent,
                ExecutionTime = (DateTime.Now - userInfo.RequestTime).TotalMilliseconds,
                Body = jsonBody,
                RequestTime = userInfo.RequestTime.ToTimeStampSeconds(),
                UserName = userInfo.MobileNumber,
                ServiceStatus = userInfo.ServiceStatus,
                ResponseModel = responseModel
            };

            var headers = new Dictionary<string, string>
            {
                { "ApiKeyInternal", $"{_apiKeyInternal}" },
                { "AppId", "6" },
                { "DeviceTypeId", "9" },
                { "AppLanguage", "fa" }
            };

            await _requestHelper2.CallServiceAsync(requestUrl, model, headers, HttpMethod.Post);
        }
    }
}


