using IdentityService.Services.IServices;
using IdentityService.Services.ViewModels;
using System.Runtime.CompilerServices;
using OmidBank.Common.Base;

namespace IdentityService.Services.Services
{
    public class CityService : BaseService, ICityService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<CityService> _logger;
        private readonly string _extraDataServiceUrl;
        private readonly string _apiKeyInternal;

        public CityService(IRequestHelper requestHelper, ILogger<CityService> logger,IConfiguration configuration)
        {
            _requestHelper = requestHelper ;
            _logger = logger ;
            _extraDataServiceUrl = configuration["ExtraDataServiceUrl"];
            _apiKeyInternal = configuration["ApiKeyInternal"];
        }

        public async Task<CityInformationDTO> GetCityById(int? cityId, CancellationToken cancellationToken)
        {
            try
            {
                if (cityId == null)
                    return new CityInformationDTO();

                var requestUrl = $"{_extraDataServiceUrl}/v2/cities/{cityId}";
                var headers = new Dictionary<string, string>
                {
                    { "AppId", "6" },
                    { "DeviceTypeId", "9" },
                    { "AppLanguage", "fa" },
                    { "ApiKeyInternal", _apiKeyInternal},
                };

                var response = await _requestHelper.GetAsync(requestUrl, headers);
                var city = JsonSerializer.Deserialize<BaseMessageResponse<CityInformationDTO>>(response, JsonHelper.JsonCamelCaseOption());
                return city.StatusCode != EResponseStatusCode.DoneSuccessfully ? new CityInformationDTO() : city.Data;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return new CityInformationDTO();
            }
        }
    }
}
