namespace IdentityService.Services.Services
{
    public class WalletService : BaseService, IWalletService
    {
        private readonly ILogger<WalletService> _logger;
        private readonly IRequestHelper _requestHelper;

        public WalletService(ILogger<WalletService> logger, IRequestHelper requestHelper)
        {
            _logger = logger;
            _requestHelper = requestHelper;
        }

        public async Task<bool> AddWallet(UserModel user, string token, string url, string apikeyInternal, CancellationToken cancellationToken)
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
                var model = new AddUserWalletModel()
                {
                    MobileNumber = user.MobileNumber,
                    UserId = user.Id,
                    WalletTypeId = 1
                };
                var response = await _requestHelper.PostAsync(requestUrl, model, headers);
                var status = JsonSerializer.Deserialize<bool>(response, JsonHelper.JsonCamelCaseOption());
                return status;
            }
            catch (Exception exp)
            {
                _logger.LogError(exp.StackTrace);
                return false;
            }
        }
    }
}
