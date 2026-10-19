//using IdentityService.Domian.DTO;
//using OmidBank.Common.InternalThirdParty.Metaboom;
//using OmidBank.Common.InternalThirdParty.Metaboom.Request;
//using OmidBank.Common.InternalThirdParty.Metaboom.Response;
//using OmidBank.Common.InternalThirdParty.ShahkarService;
//using OmidBank.Common.InternalThirdParty.ShahkarService.Request;
//using OmidBank.Common.InternalThirdParty.ShahkarService.Response;
//using OmidBank.Common.InternalThirdParty.SmsService;
//using OmidBank.Common.InternalThreadParty.OmidTech;
//using OmidBank.Common.InternalThreadParty.OmidTech.Response;
//using XAct;

//namespace IdentityService.Services.Services
//{
//    public class ShahkarService : BaseService, IShahkarService
//    {
//        private readonly ILogger _logger;
//        private readonly string _uidBaseServiceUrl;
//        private readonly string _uidBusinessId;
//        private readonly string _uidBusinessToken;
//        private readonly string _logServiceUrl;
//        private readonly string _baseUrlMetaBoom;
//        private readonly string _basUrlDrApi;
//        private readonly string _apiKey;
//        private readonly string _baseUrlOmidTech;
//        private readonly string _usernameNoavaran;
//        private readonly string _passwordNoavaran;
//        private readonly IRequestHelper _requestHelper;
//        private readonly AstanServiceProvider _shahkarServiceProvider;
//        private readonly int _shahkarServiceType;
//        private readonly IConfiguration _configuration;


//        public ShahkarService(IConfiguration configuration, ILogger<ShahkarService> logger, IRequestHelper requestHelper)
//        {
//            _logger = logger;
//            _configuration = configuration;
//            _uidBaseServiceUrl = configuration["UidBaseServiceUrl"];
//            _uidBusinessId = configuration["UidBusinessId"];
//            _uidBusinessToken = configuration["UidBusinessToken"];
//            _logServiceUrl = configuration["LogServiceUrl"];
//            _requestHelper = requestHelper;
//            _baseUrlMetaBoom = configuration["BaseUrlMetaBoom"];
//            _basUrlDrApi = configuration["BasUrlDrApi"];
//            _apiKey = configuration["ApiKeyOmidTech"];
//            _baseUrlOmidTech = configuration["BaseOmidTechUrl"];
//            _usernameNoavaran = configuration["UsernameNoavaran"];
//            _passwordNoavaran = configuration["PasswordNoavaran"];
//            _shahkarServiceProvider = new AstanServiceProvider(_basUrlDrApi, _apiKey, _baseUrlOmidTech, _usernameNoavaran, _passwordNoavaran);
//            _shahkarServiceType = int.Parse(configuration["ShahkarServiceType"]);

//        }

//        public async Task<ShahkarResViewModel> ValidateIdentityByShahkar(ShahkarParameterViewModel model, RequestInfo clientInfo, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var requestCheckShahkar = new ValidateIdentityByShahkarRequest();
//                requestCheckShahkar.MobileNumber = model.MobileNumber;
//                requestCheckShahkar.NationalCode = model.NationalId;
//                var result = new ShahkarBaseResponseViewModel();
//                MetaboomProvider metaboomProvider = new MetaboomProvider(_configuration);
//                switch (_shahkarServiceType)
//                {
//                    case 1://Metaboom
//                        result = await metaboomProvider.ValidateIdentityByShahkar(requestCheckShahkar, cancellationToken);
//                        _logger.LogInformation("Here 1" + JsonSerializer.Serialize(result));
//                        break;
//                    case 2://DrApi
//                        result = await _shahkarServiceProvider.ValidateIdentityByShahkar(requestCheckShahkar, cancellationToken);
//                        _logger.LogInformation("Here 2" + JsonSerializer.Serialize(result));
//                        break;
//                    default:
//                        break;
//                }
//                if (result.response == 200)
//                    return new ShahkarResViewModel() { response = 200 };
//                else
//                    return null;
//            }
//            catch (Exception exp)
//            {
//                _logger.LogError(exp.Message);
//                return null;
//            }
//            finally
//            {
//                string url = $"{model.Url}/Shahkar/ServiceMatchingId";
//                AddLog(clientInfo, model, url, cancellationToken);
//            }
//        }

//        private async Task AddLog(RequestInfo userInfo, object body, string url, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var requestUrl = $"{_logServiceUrl}/v1/log/add";
//                var jsonBody = string.Empty;
//                if (body != null)
//                    jsonBody = JsonSerializer.Serialize(body, JsonHelper.JsonCamelCaseOption());

//                if (userInfo.ResponseModel != null)
//                    userInfo.ResponseModel = JsonSerializer.Serialize(userInfo.ResponseModel, JsonHelper.JsonCamelCaseOption());

//                var headers = new Dictionary<string, string>
//                {
//                    { "AppId", "6" },
//                    { "DeviceTypeId", "9" },
//                    { "AppLanguage" , "fa"}
//                };

//                var model = new
//                {
//                    Url = url,
//                    userInfo.IP,
//                    userInfo.Token,
//                    userInfo.Version,
//                    userInfo.AppId,
//                    userInfo.DeviceTypeId,
//                    userInfo.DeviceInfo,
//                    userInfo.UserAgent,
//                    ExecutionTime = (DateTime.Now - userInfo.RequestTime).TotalMilliseconds,
//                    Body = jsonBody,
//                    RequestTime = userInfo.RequestTime.ToTimeStampSeconds(),
//                    UserName = userInfo.MobileNumber,
//                    ServiceStatus = userInfo.ServiceStatus,
//                    ResponseModel = userInfo.ResponseModel
//                };
//                await _requestHelper.PostAsync(requestUrl, model, headers);
//            }
//            catch (Exception exp)
//            {
//                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
//            }
//        }


//        public async Task<ShahkarBaseResponseViewModel> ValidateIdentityByShahkar123(ValidateIdentityByShahkarRequest request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var result = new ShahkarResponse();
//                var finalResult = new ShahkarBaseResponseViewModel();

//                var getToken = await GetToken();
//                if (getToken == null)
//                    return null;

//                var sendShahkarRequest = new ShahkarRequest()
//                {
//                    Client = _configuration["Metaboom:Client"],
//                    Nid = _configuration["Metaboom:Nid"]
//                };

//                sendShahkarRequest.ResourceParams = new ResourceParam()
//                {
//                    MobileNumber = request.MobileNumber,
//                    NationalCode = request.NationalCode,
//                };

//                _logger.LogInformation("Here requestGetToken " + JsonSerializer.Serialize(sendShahkarRequest,JsonHelper.JsonCamelCaseOption()));


//                var handler = new HttpClientHandler
//                {
//                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
//                };
//                using var client = new HttpClient(handler);
//                var requestProvider = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["Metaboom:BaseUrl"]}/tls-shahkar");
//                requestProvider.Headers.Add("x-access-token", $"{getToken}");
//                var content = new StringContent(JsonSerializer.Serialize(sendShahkarRequest, JsonHelper.JsonCamelCaseOption()), Encoding.UTF8, "application/json");
//                requestProvider.Content = content;
//                var responseProvider = await client.SendAsync(requestProvider);

//                _logger.LogInformation("Here responseProvider " + responseProvider);

//                if (responseProvider.StatusCode == HttpStatusCode.OK)
//                {
//                    var response = await responseProvider.Content.ReadAsStringAsync();

//                    _logger.LogInformation("Here response " + JsonSerializer.Serialize(response));

//                    result = JsonSerializer.Deserialize<ShahkarResponse>(response, JsonHelper.JsonCamelCaseOption());
//                    if (result.result.isMatched == "true")
//                    {
//                        finalResult.response = 200;
//                        return finalResult;
//                    }
//                    else
//                        return null;
//                }
//                else
//                    return null;
//            }
//            catch (Exception e)
//            {
//                return null;
//            }
//        }

//        private async Task<string> GetToken()
//        {
//            try
//            {
//                var handler = new HttpClientHandler
//                {
//                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
//                };

//                using var client = new HttpClient(handler);
//                var requestProvider = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["Metaboom:BaseUrl"]}/token/generate/client");
//                var requestGetToken = new GetTokenMetaboomRequest
//                {
//                    client = _configuration["Metaboom:Client"],
//                    signature = _configuration["Metaboom:Signature"],
//                };
//                requestGetToken.scopes = new List<string> { _configuration["Metaboom:Scopes"] };

//                _logger.LogInformation("Here requestGetToken " + JsonSerializer.Serialize(requestGetToken));

//                var content = new StringContent(JsonSerializer.Serialize(requestGetToken), Encoding.UTF8, "application/json");
//                requestProvider.Content = content;
//                var responseProvider = await client.SendAsync(requestProvider);
//                if (responseProvider.IsSuccessStatusCode)
//                {
//                    var response = await responseProvider.Content.ReadAsStringAsync();
//                    _logger.LogInformation("Here 1245" + JsonSerializer.Serialize(response));

//                    var finalResult = JsonSerializer.Deserialize<ResponseGetToken>(response);
//                    return finalResult.result.token.access;
//                }
//                else
//                    return null;
//            }
//            catch (Exception e)
//            {
//                return null;
//            }
//        }


//    }
//}
