

namespace IdentityService.Services.Services
{
    public class OrganizationService : BaseService, IOrganizationService
    {
        private readonly ILogger<PannelAuthService> _logger;
        private readonly IRequestHelper _requestHelper;

        public OrganizationService(ILogger<PannelAuthService> logger, IRequestHelper requestHelper)
        {
            _logger = logger;
            _requestHelper = requestHelper;
        }

        public async Task<OrganizationModel> GetOrganizationAsync(string token, int organizationId,string url, CancellationToken cancellationToken)
        {
            try
            {
                var requestUrl = $"{url}/admin/Organization/GetById?id={organizationId}";
                var headers = new Dictionary<string, string>
                {
                    { "AppId", "6" },
                    { "DeviceTypeId", "9" },
                    { "Authorization", $"Basic {token}" },
                    { "AppLanguage" , "fa"}
                };
                var response = await _requestHelper.GetAsync(requestUrl, headers);
                var organization = JsonSerializer.Deserialize<OrganizationModel>(response, JsonHelper.JsonCamelCaseOption());
                return organization;

            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
        }
    }
}
