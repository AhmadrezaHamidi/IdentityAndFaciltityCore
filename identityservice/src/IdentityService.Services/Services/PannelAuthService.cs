namespace IdentityService.Services.Services
{
    public class PannelAuthService : BaseService, IPannelAuthService
    {
        private readonly ILogger<PannelAuthService> _logger;
        private readonly IRequestHelper _requestHelper;
        private readonly IJwtService _jwtService;
        private readonly string _jwtTokenEncryptorKey;        

        public PannelAuthService(ILogger<PannelAuthService> logger, IConfiguration configuration, IJwtService jwtService, IRequestHelper requestHelper)
        {
            _logger = logger;
            _requestHelper = requestHelper;
            _jwtService = jwtService;
            _jwtTokenEncryptorKey = configuration["JwtTokenEncryptorKey"];
        }

        public async Task<Tuple<bool, string, long, string, int>> PanelAuthenticate(string token, string controllerName, string actionName, string url, CancellationToken cancellationToken)
        {
            try
            {
                var requestUrl = $"{url}/admin/v1/Account/PanelAuthentication";
                var model = new
                {
                    Token = token,
                    ControllerName = controllerName,
                    ActionName = actionName
                };
                var headers = new Dictionary<string, string>
                {
                    { "AppId", "6" },
                    { "DeviceTypeId", "9" },
                    { "AppLanguage" , "fa"}
                };

                var response = await _requestHelper.PostAsync(requestUrl, model, headers);               
                var user = JsonSerializer.Deserialize<Tuple<bool, string, long, string, int>>(response, JsonHelper.JsonCamelCaseOption());

                return user;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return new Tuple<bool, string, long, string, int>(false, "مشکلی در برقراری ارتباط با سرور رخ داده است", 0, "", 401);
            }
        }

        public async Task<Tuple<bool, string, long, string, int>> PanelAuthenticateForToken(string token, string url, CancellationToken cancellationToken)
        {
            try
            {
                var valid = _jwtService.ValidateToken(token);
                var queryResult = new Tuple<bool, string, long, string, int>(false, "دسترسی این قسمت از سرویس برای رول کاربر تعریف نشده است", 0, "", valid.Item4);//valid-Message-UserId-UserName-valid Code for panel
                if (!valid.Item1)
                {
                    queryResult = new Tuple<bool, string, long, string, int>(valid.Item1, valid.Item2, 0, "", valid.Item4);
                    return queryResult;
                }
                queryResult = new Tuple<bool, string, long, string, int>(true, "مجوز صادر شد", valid.Item3.UserId, valid.Item3.UserName, 702);
                return queryResult;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return new Tuple<bool, string, long, string, int>(false, "مشکلی در برقراری ارتباط با سرور رخ داده است", 0, "", 401);
            }
        }
    }
}