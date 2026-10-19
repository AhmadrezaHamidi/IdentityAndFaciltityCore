using IdentityService.Domain.Entity.UserManagement.LoginLogOutHistory;
using IdentityService.Domain.IRepositories;
using IdentityService.Services.IServices;
using IdentityService.Services.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OmidBank.Common.Cache;
using OmidBank.Common.Extentions;
using OmidBank.Common.ExternalThirdParty.Tosan;
using OmidBank.Common.ExternalThirdParty.Tosan.Other;
using OmidBank.Common.ExternalThirdParty.Tosan.TicketGeneration;
using OmidBank.Common.ExternalThirdParty.Tosan.TicketGeneration.Request;
using OmidBank.Common.ExternalThirdParty.Tosan.User;
using OmidBank.Common.ExternalThirdParty.Tosan.User.Request;
using OmidBank.Common.InternalThreadParty.OmidTech;
using OmidBank.Common.InternalThreadParty.OmidTech.Request;

namespace IdentityService.Application.Handler.UserManagement;

public class LoginUMHandler : IBaseHandler<LoginUMRequest, BaseMessageResponse<LoginUMHResponse>>
{
    private readonly IConfiguration _configuration;

    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly bool _isDevelopment;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly TosanUserProvider _tosanUserProvider;
    private readonly TosanTicketGenerationProvider _tosanTicketGenerationProvider;
    private readonly ISmsService _smsService;
    private readonly string _messageServiceUrl, _apiKeyInternal;
    private readonly ILoginLogoutHistoryRepository _loginLogoutHistoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly HttpClient _httpClient;

    public LoginUMHandler(
        IConfiguration configuration,
        ILogger<RequestResponseLoggingMiddleware> logger,
        IRedisCacheManager redisCacheManager,
        TosanUserProvider tosanUserProvider,
        TosanTicketGenerationProvider tosanTicketGenerationProvider,
        ISmsService smsService,
        ILoginLogoutHistoryRepository loginLogoutHistoryRepository,
        IUserRepository userRepository,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _isDevelopment = bool.Parse(configuration["IsDevelopment"]);
        _logger = logger;
        _redisCacheManager = redisCacheManager;
        _tosanUserProvider = tosanUserProvider;
        _tosanTicketGenerationProvider = tosanTicketGenerationProvider;
        _smsService = smsService;
        _loginLogoutHistoryRepository = loginLogoutHistoryRepository;
        _messageServiceUrl = configuration["MessageServiceUrl"];
        _apiKeyInternal = configuration["ApiKeyInternal"];
        _userRepository = userRepository;
        _httpClient = httpClient;
    }

    public async Task<BaseMessageResponse<LoginUMHResponse>> Handle(LoginUMRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<LoginUMHResponse>(EResponseStatusCode.DoneSuccessfully, new());
        var omidTechProvider = new OmidTechProvider(_configuration["BaseOmidTechUrl"], _configuration["ApiKeyOmidTech"], _httpClient);

        var loginResponse = await _tosanUserProvider.LoginStaticAsync(new LoginStaticRequest(null)
        {
            Username = request.UserName,
            Password = ***;

        if (loginResponse.Exception is not null)
        {
            if (!_isDevelopment)
            {
                var sendSmsRequest = new SendActivitionCodeOmidTechViewModel()
                {
                    MobileNumber = request.RequestInfo.MobileNumber,
                    Scheduler = null,
                    Code = null,
                    Url = _messageServiceUrl,
                    ApiKeyInternal = _apiKeyInternal,
                    Message = $"امید بانک سپه \n ورود ناموفق به خدمات بانکی \n {TimeStampExtention.GetDatePersian(DateTime.Now)} {TimeStampExtention.GetTimePersian(DateTime.Now)}",
                };
                _smsService.SendActivationCodeFromThirdPartyOmidTech(sendSmsRequest, cancellationToken);
            }

            response = MessageResponse.GenerateMessageResponse<LoginUMHResponse>(EResponseStatusCode.BadRequest);
            response.Message = loginResponse.Exception.TranslateMessage;
            await _loginLogoutHistoryRepository.AddAsync(new()
            {
                Status = 0,
                TypeAction = 1,
                Ip = request.RequestInfo.IP,
                DeviceInfo = request.RequestInfo.DeviceInfo,
                UserId = request.CurrentUser!.Id,
            }, cancellationToken);
            await _loginLogoutHistoryRepository.SaveChangeAsync(cancellationToken);
            return response;
        }

        if (loginResponse.Result.Code != request.CurrentUser.NationalCode)
        {
            response = MessageResponse.GenerateMessageResponse<LoginUMHResponse>(EResponseStatusCode.BadRequest);
            await _loginLogoutHistoryRepository.AddAsync(new()
            {
                Status = 0,
                TypeAction = 1,
                Ip = request.RequestInfo.IP,
                DeviceInfo = request.RequestInfo.DeviceInfo,
                UserId = request.CurrentUser!.Id,
            }, cancellationToken);
            return response;
        }
        await _loginLogoutHistoryRepository.SaveChangeAsync(cancellationToken);
        var customerCif = loginResponse.Result.Cif is not null ? long.Parse(loginResponse.Result.Cif) : 0;

        var guid = Guid.NewGuid().ToString();
        response.Data.AccessToken = guid;
        response.Data.RefreshToken = guid;
        response.Data.BiometricToken = guid;
        response.Data.BankCustomerNumber = customerCif;
        response.Data.StatusCode = 1;
        response.Data.IsBankCustomer = true;
        var userData = await _userRepository.FindById(request.CurrentUser.Id, cancellationToken);
        if (userData.BankCustomerNumber is null && customerCif != 0)
        {
            await _userRepository.UpdateUserCustomerNumber(request.CurrentUser.Id, customerCif, cancellationToken);
            await _userRepository.SaveChangeAsync(cancellationToken);
        }

        //if temp null bood
        if (string.IsNullOrEmpty(loginResponse.Result.TemporarySessionId))
        {
            _redisCacheManager.Write(string.Format(CacheKey.TosanUserSessionId, request.RequestInfo.Token), loginResponse.Result.SessionId, Enums.ECacheTime.OneQuarter);
            response.Data.HasTwoFactorAuthentication = false;

            if (!_isDevelopment)
            {
                var sendSmsRequest = new SendActivitionCodeOmidTechViewModel()
                {
                    MobileNumber = request.RequestInfo.MobileNumber,
                    Scheduler = null,
                    Code = null,
                    Url = _messageServiceUrl,
                    ApiKeyInternal = _apiKeyInternal,
                    Message = $"امید بانک سپه \n ورود موفق به خدمات بانکی \n {TimeStampExtention.GetDatePersian(DateTime.Now)} {TimeStampExtention.GetTimePersian(DateTime.Now)}",
                };
                _smsService.SendActivationCodeFromThirdPartyOmidTech(sendSmsRequest, cancellationToken);
                await _loginLogoutHistoryRepository.AddAsync(new()
                {
                    Status = 1,
                    TypeAction = 1,
                    Ip = request.RequestInfo.IP,
                    DeviceInfo = request.RequestInfo.DeviceInfo,
                    UserId = request.CurrentUser!.Id,
                }, cancellationToken);
            }
            await _loginLogoutHistoryRepository.SaveChangeAsync(cancellationToken);

            return response;
        }
        else
        {
            var requestDoTicketGeneration = new DoTicketGenerationRequest(loginResponse.Result.TemporarySessionId)
            {
                MediaType = TicketMediaType.SMS,
                ServiceInfoBean = new ServiceInfoBean()
                {
                    ServiceResourceDetail = new ServiceResourceDetailBean()
                    {
                        ModernServcieParameterResourceType = ModernServcieParameterResourceType.CHANNEL,
                        Value = "YAGHUT-MOBILE-APPLICATION"
                    },
                    TicketServiceType = TicketServiceType.TWO_PHASE_LOGIN
                }
            };
            var resultTicketGeneration = await _tosanTicketGenerationProvider.DoTicketGenerationAsync(requestDoTicketGeneration, cancellationToken);
            if (resultTicketGeneration.Exception is not null)
            {
                if (!_isDevelopment)
                {
                    var sendSmsRequest = new SendActivitionCodeOmidTechViewModel()
                    {
                        MobileNumber = request.RequestInfo.MobileNumber,
                        Scheduler = null,
                        Code = null,
                        Url = _messageServiceUrl,
                        ApiKeyInternal = _apiKeyInternal,
                        Message = $"امید بانک سپه \n ورود ناموفق به خدمات بانکی \n {TimeStampExtention.GetDatePersian(DateTime.Now)} {TimeStampExtention.GetTimePersian(DateTime.Now)}",
                    };
                     _smsService.SendActivationCodeFromThirdPartyOmidTech(sendSmsRequest, cancellationToken);

                }

                response = MessageResponse.GenerateMessageResponse<LoginUMHResponse>(EResponseStatusCode.BadRequest);
                response.Message = loginResponse.Exception.TranslateMessage;
                await _loginLogoutHistoryRepository.AddAsync(new()
                {
                    Status = 0,
                    TypeAction = 1,
                    Ip = request.RequestInfo.IP,
                    DeviceInfo = request.RequestInfo.DeviceInfo,
                    UserId = request.CurrentUser!.Id,
                }, cancellationToken);

                return response;
            }
            await _loginLogoutHistoryRepository.SaveChangeAsync(cancellationToken);
            //var temporarySessionIdKey = request.RequestInfo.Token + "_temp";
            // save temporarySessionId 
            _redisCacheManager.Write(string.Format(CacheKey.TosanUserSessionId, request.RequestInfo.Token), loginResponse.Result.TemporarySessionId, Enums.ECacheTime.ThreeMinutes);
            response.Data.HasTwoFactorAuthentication = true;
            await _loginLogoutHistoryRepository.AddAsync(new()
            {
                Status = 0,
                TypeAction = 1,
                Ip = request.RequestInfo.IP,
                DeviceInfo = request.RequestInfo.DeviceInfo,
                UserId = request.CurrentUser!.Id,
            }, cancellationToken);
            await _loginLogoutHistoryRepository.SaveChangeAsync(cancellationToken);
            return response;
        }
    }
}

public record LoginUMRequest(
    [Required] string UserName,
    [Required] string Password,
    [property: System.Text.Json.Serialization.JsonIgnore]
    string EncrytedData) : BaseCommand<BaseMessageResponse<LoginUMHResponse>>;

public class LoginUMHResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string BiometricToken { get; set; }
    public int StatusCode { get; set; }
    public bool IsBankCustomer { get; set; }
    public long? BankCustomerNumber { get; set; }
    public bool HasTwoFactorAuthentication { get; set; }
}