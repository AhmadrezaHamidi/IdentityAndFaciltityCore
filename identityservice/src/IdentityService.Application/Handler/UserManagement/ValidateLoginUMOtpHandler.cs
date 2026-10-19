using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmidBank.Common.Cache;
using OmidBank.Common.Extentions;
using OmidBank.Common.ExternalThirdParty.Tosan.Global.Request;
using OmidBank.Common.ExternalThirdParty.Tosan.Global;
using OmidBank.Common.ExternalThirdParty.Tosan.User;
using OmidBank.Common.ExternalThirdParty.Tosan.User.Request;
using OmidBank.Common.InternalThreadParty.OmidTech;
using OmidBank.Common.InternalThreadParty.OmidTech.Request;
using System.Text.Json.Serialization;
using IdentityService.Domain.IRepositories;
using IdentityService.Services.ViewModels;
using IdentityService.Services.IServices;
using System.Net.Http;

namespace IdentityService.Application.Handler.UserManagement
{
    public class TwoPhaseloginHandler : IBaseHandler<TwoPhaseloginDataRequest, BaseMessageResponse<TwoPhaseloginResponse>>
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly bool _isDevelopment;
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly TosanUserProvider _tosanUserProvider;
        private readonly IUserRepository _userRepository;
        private TosanGlobalProvider _tosanGlobalProvider;
        private readonly ISmsService _smsService;
        private readonly string _messageServiceUrl, _apiKeyInternal;
        private readonly HttpClient _httpClient;




        public TwoPhaseloginHandler(
            IConfiguration configuration,
            ILogger<RequestResponseLoggingMiddleware> logger,
            IRedisCacheManager redisCacheManager,
            TosanUserProvider tosanUserProvider,
            IUserRepository userRepository,
            TosanGlobalProvider tosanGlobalProvider,
            ISmsService smsService,
            HttpClient httpClient
            )
        {
            _configuration = configuration;
            _isDevelopment = bool.Parse(configuration["IsDevelopment"]);
            _logger = logger;
            _redisCacheManager = redisCacheManager;
            _tosanUserProvider = tosanUserProvider;
            _userRepository = userRepository;
            _tosanGlobalProvider = tosanGlobalProvider;
            _smsService = smsService;
            _httpClient = httpClient;
            _messageServiceUrl = configuration["MessageServiceUrl"];
            _apiKeyInternal = configuration["ApiKeyInternal"];

        }

        public async Task<BaseMessageResponse<TwoPhaseloginResponse>> Handle(TwoPhaseloginDataRequest request, CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<TwoPhaseloginResponse>(EResponseStatusCode.DoneSuccessfully, new());
            long? customerCif = 0;
            var mobile = new List<string>()
            {
            request.RequestInfo.MobileNumber
            };
            var omidTechProvider = new OmidTechProvider(_configuration["BaseOmidTechUrl"], _configuration["ApiKeyOmidTech"], _httpClient);
            //var temporarySessionIdKey = request.RequestInfo.Token + "_temp";
            var sessionIdUser = _redisCacheManager.Read(string.Format(CacheKey.TosanUserSessionId, request.RequestInfo.Token));
            if (sessionIdUser is null)
            {
                response = MessageResponse.GenerateMessageResponse<TwoPhaseloginResponse>(EResponseStatusCode.UnAuthorized);
                return response;
            }
            var requestTwoPhase = new TwoPhaseloginRequest(sessionIdUser.ToString())
            {
                Ticket = request.OtpCode,
                UseBaseUrlSystem = false
            };
            var twoPhaseloginResponse = await _tosanUserProvider.TwoPhaseloginAsync(requestTwoPhase, cancellationToken);
            if (twoPhaseloginResponse.Exception is not null || twoPhaseloginResponse is null)
            {
                response = MessageResponse.GenerateMessageResponse<TwoPhaseloginResponse>(EResponseStatusCode.CallErrorTosan);
                return response;
            }
            var userDetail = await _userRepository.FindById(request.CurrentUser.Id, cancellationToken);
            if (userDetail.BankCustomerNumber is not null)
                customerCif = userDetail.BankCustomerNumber;

            var guid = Guid.NewGuid().ToString();
            response.Data.AccessToken = guid;
            response.Data.RefreshToken = guid;
            response.Data.BiometricToken = guid;
            response.Data.StatusCode = 1;
            response.Data.BankCustomerNumber = customerCif;
            response.Data.IsBankCustomer = true;
            _redisCacheManager.Write(string.Format(CacheKey.TosanUserSessionId, request.RequestInfo.Token), twoPhaseloginResponse.Result.SessionId
                , Enums.ECacheTime.HalfAnHour);
            if (!_isDevelopment)
            {
                var sendSmsRequest = new SendActivitionCodeOmidTechViewModel()
                {
                    MobileNumber = request.RequestInfo.MobileNumber,
                    Scheduler = null,
                    Code = null,
                    Url = _messageServiceUrl,
                    ApiKeyInternal = _apiKeyInternal,
                    Message = $"امید بانک سپه \n ورود به خدمات بانکی \n {TimeStampExtention.GetDatePersian(DateTime.Now)} {TimeStampExtention.GetTimePersian(DateTime.Now)}",
                };
                await _smsService.SendActivationCodeFromThirdPartyOmidTech(sendSmsRequest, cancellationToken);
            }
            return response;
        }
    }
}
public record TwoPhaseloginDataRequest([Required] string OtpCode, [property: JsonIgnore] string EncrytedData) :
    BaseCommand<BaseMessageResponse<TwoPhaseloginResponse>>;
public class TwoPhaseloginResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string BiometricToken { get; set; }
    public int StatusCode { get; set; }
    public bool IsBankCustomer { get; set; }
    public long? BankCustomerNumber { get; set; }
}
