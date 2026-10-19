using IdentityService.Services.ViewModels;

namespace IdentityService.Services.Services
{
    public class CivilRegisteryService : BaseService, ICivilRegistryServices
    {
        private readonly ILogger _logger;
        private readonly string _uidBaseServiceUrl;
        private readonly string _uidBusinessId;
        private readonly string _uidBusinessToken;

        public CivilRegisteryService(IConfiguration configuration, ILogger<CivilRegisteryService> logger)
        {
            _logger = logger;
            _uidBaseServiceUrl = configuration["UidBaseServiceUrl"];
            _uidBusinessId = configuration["UidBusinessId"];
            _uidBusinessToken = configuration["UidBusinessToken"];
        }

        public CivilRegisteryResultViewModel PersonValidateByCivilRegistery(CivilRegisteryParameterViewModel model, CancellationToken cancellationToken)
        {
            var client = new RestClient(_uidBaseServiceUrl + "/inquiry/person");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json",
                "{\n \"requestContext\" : " +
                "{\n \"apiInfo\" : " +
                "{\n  \"businessId\" : \"" + _uidBusinessId + "\",\n \"businessToken\" : \"" + _uidBusinessToken + "\"\n }\n },\n  " +
                      "\"nationalId\" : \"" + model.NationalId + "\",\n    \"birthDate\" : \"" + model.BirthDate + "\"\n}",
                ParameterType.RequestBody);           
            var response = client.Execute(request);
            var result = JsonSerializer.Deserialize<CivilRegisteryResultViewModel>(response.Content, JsonHelper.JsonCamelCaseOption());
            if (result?.responseContext?.status?.code == 0)
                return result;

            throw new Exception("خطا در سرویس ثبت احوال!");
        }

        public async Task<GetSearchRealCustomerViewModel> SearchRealCustomerByNationalCode(string nationalCode, string url, string apiKeyApiBankSepah, CancellationToken cancellationToken)
        {
            var result = new GetSearchRealCustomerViewModel();
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/Api/SearchRealCustomerByNationalCode");
                request.Headers.Add("ApiKey", $"{apiKeyApiBankSepah}");
                var sendModel = new
                {
                    NationalCode = nationalCode
                };

                var content = new StringContent(JsonSerializer.Serialize(sendModel, JsonHelper.JsonCamelCaseOption()), Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                var res = await response.Content.ReadAsStringAsync();
                var finalResult = JsonSerializer.Deserialize<ResSearchRealCustomerByNationalCodeViewModel>(res, JsonHelper.JsonCamelCaseOption());
                var xx = new GetSearchRealCustomerViewModel()
                {
                    IsSuccess = finalResult.IsSuccess,
                    CustomerNum = finalResult.ResultData.CustomerNumber

                };
                return xx;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
        }

    }
}
