namespace IdentityService.Services.Services
{
    public class LogService : BaseService, ILogService
    {
        private readonly string _logServiceUrl;
        private readonly ILogger<LogService> _logger;
        private readonly IRequestHelper _requestHelper;

        public LogService(ILogger<LogService> logger, IConfiguration configuration, IRequestHelper requestHelper)
        {
            _logger = logger;
            _logServiceUrl = configuration["LogServiceUrl"];
            _requestHelper = requestHelper;
        }

        public async Task AddLog(RequestInfo userInfo, object body, string url, CancellationToken cancellationToken)
        {
            try
            {
                //var requestUrl = $"{_logServiceUrl}/v1/log/add";
                var jsonBody = string.Empty;
                if (body != null)
                    jsonBody = JsonSerializer.Serialize(body, JsonHelper.JsonCamelCaseOption());

                var model = new
                {
                    Url = userInfo.RequestUrl,
                    userInfo.IP,
                    userInfo.Token,
                    userInfo.Version,
                    userInfo.AppId,
                    userInfo.DeviceTypeId,
                    userInfo.DeviceInfo,
                    userInfo.UserAgent,
                    ExecutionTime = (DateTime.Now - userInfo.RequestTime).TotalMilliseconds,
                    Body = jsonBody,
                    RequestTime = userInfo.RequestTime.ToTimeStampSeconds(),
                    UserName = userInfo.MobileNumber,
                    ServiceStatus=userInfo.ServiceStatus,
                };

                var headers = new Dictionary<string, string>();

                headers.Add("AppId", userInfo.AppId.ToString());
                headers.Add("DeviceTypeId", userInfo.DeviceTypeId.ToString());
                headers.Add("AppLanguage", "fa");
                await _requestHelper.PostAsync(url, model, headers);
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
            }
        }
    }
}
