using IdentityService.Services.ViewModels;
using OmidBank.Common.Cache;
using OmidBank.Common.InternalThirdParty.Ateye.Request;
using OmidBank.Common.InternalThirdParty.Ateye.Response;
using OmidBank.Common.InternalThirdParty.SmsService;
using OmidBank.Common.InternalThirdParty.SmsService.Request;
using OmidBank.Common.InternalThirdParty.SmsService.Response;
using OmidBank.Common.InternalThreadParty.OmidTech;
using OmidBank.Common.InternalThreadParty.OmidTech.Request;
using OmidBank.Common.InternalThreadParty.OmidTech.Response;

namespace IdentityService.Services.Services
{
    public class SmsService : BaseService, ISmsService
    {
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly IRequestHelper _requestHelper;
        public readonly string _smsMessage;
        private readonly OmidTechProvider _omidTechProvider;
        private int _smsServiceType;
        private readonly IConfiguration _configuration;
        private string _baseAddress, _sourceAddress, _apikey;
        private IOptions<SmsProviderOptions> _smsProviderOptions;
        private IRedisCacheManager _redisCacheManager;
        private HttpClient _httpClient;
        private ISmsThrottleService _smsThrottleService;

        public SmsService(ILogger<RequestResponseLoggingMiddleware> logger, IRequestHelper requestHelper, IConfiguration configuration
              , IOptions<SmsProviderOptions> smsProviderOptions, IRedisCacheManager redisCacheManager, HttpClient httpClient,ISmsThrottleService smsThrottleService)
        {
            _logger = logger;
            _requestHelper = requestHelper;
            _smsMessage = configuration["SmsMessage"];
            _httpClient = httpClient;
            _omidTechProvider = new OmidTechProvider(configuration["BaseUrlApiBankSepah"], configuration["ApiKeyApiBankSepah"], _httpClient);
            _smsServiceType = int.Parse(configuration["SmsServiceType"]);
            _configuration = configuration;
            _smsProviderOptions = smsProviderOptions;
            _redisCacheManager = redisCacheManager;
            _httpClient = httpClient;
            _smsThrottleService = smsThrottleService;
        }

        public async Task<SmsThirdPartyResponseModel> SendActivationCodeFromThirdParty(string mobileNumber, string code, string url, string userName, CancellationToken cancellationToken)
        {

            try
            {
                var requestUrl = $"{url}/v1/Sms/SendMessage";
                var headers = new Dictionary<string, string>
                {
                    { "AppId", "6" },
                    { "DeviceTypeId", "9" },
                    { "AppLanguage" , "fa"},
                    { "DeviceId" ,$"{userName}"}
                };

                var mobile = new List<string>();
                mobile.Add(mobileNumber);

                var model = new SmsThirdPartyRequestModel()
                {
                    MobileNumbers = mobile,
                    //MessageBody = $"کد تایید امید بانک :{Environment.NewLine}کد:{code}",
                    MessageBody = code,
                    AllowDelay = 1
                };
                var response = await _requestHelper.PostAsync(requestUrl, model, headers);
                var messageResponse = JsonSerializer.Deserialize<List<SmsThirdPartyResponseModel>>(response, JsonHelper.JsonCamelCaseOption());
                return messageResponse[0];
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
        }

        public async Task<bool> SendActivationCodeFromThirdPartyOmidTech(SendActivitionCodeOmidTechViewModel viewModel
            , CancellationToken cancellationToken)
        {
            try
            {

                ResponseSendSms result;
                SmsServiceProvider _smsServiceProvider;
                _smsServiceProvider = new SmsServiceProvider(
    _configuration["UserNameAteye"], _configuration["PasswordAteye"], _configuration, _smsProviderOptions, _redisCacheManager, _httpClient, _smsThrottleService);
                var mobile = new List<string>();
                mobile.Add(viewModel.MobileNumber);
                var requestSendSms = new OmidBank.Common.InternalThreadParty.OmidTech.Request.SendMessageRequest();
                requestSendSms.MobileNumbers = mobile;
                requestSendSms.MessageBody = viewModel.Message is null ? $"{_smsMessage} {viewModel.Code}" : viewModel.Message;
                requestSendSms.SmsType = "omidBoom";
                requestSendSms.Scheduler = viewModel.Scheduler;
                requestSendSms.TerminalType = 18;
                result = await _smsServiceProvider.SmsSender(requestSendSms, cancellationToken);
                return result.IsSuccess;

                //switch (_smsServiceType)
                //{
                //    case 1://OmidTech
                //        result = await _smsServiceProvider.OmidTech(requestSendSms, cancellationToken);
                //        _logger.LogInformation("Here 1" + JsonSerializer.Serialize(result));
                //        break;
                //    case 2://OmidSystem
                //        result = await _smsServiceProvider.OmidSystem(requestSendSms, cancellationToken);
                //        _logger.LogInformation("Here 2" + JsonSerializer.Serialize(result));
                //        break;
                //    case 3://Ateyeh
                //        result = await _smsServiceProvider.AteyePayment(requestSendSms, cancellationToken);
                //        _logger.LogInformation("Here 3" + JsonSerializer.Serialize(result));
                //        break;
                //    case 4://arad
                //        result = await _smsServiceProvider.Aradvas(requestSendSms, cancellationToken);
                //        _logger.LogInformation("Here 4" + JsonSerializer.Serialize(result));
                //        break;
                //    default:
                //        result = new ResponseSendSms() { IsSuccess = false };
                //        break;
                //}
                //return result.IsSuccess;
            }
            catch (Exception exp)
            {
                _logger.LogInformation(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
        }

    }
}
