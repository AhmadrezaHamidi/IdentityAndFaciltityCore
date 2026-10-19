namespace IdentityService.Services.Services
{
    public class AuthKycService : BaseService, IAuthKycService
    {
        private readonly ILogger _logger;
        private readonly string _uidBaseServiceUrl;
        private readonly string _uidBusinessId;
        private readonly string _uidBusinessToken;

        public AuthKycService(IConfiguration configuration, ILogger<AuthKycService> logger)
        {
            _logger = logger;
            _uidBaseServiceUrl = configuration["UidAuthBaseServiceUrl"];
            _uidBusinessId = configuration["UidBusinessId"];
            _uidBusinessToken = configuration["UidBusinessToken"];
        }

        public AuthKycResultViewModel AuthKycCheckByUid(AuthKycParameterViewModel model, CancellationToken cancellationToken)
        {
            var client = new RestClient(_uidBaseServiceUrl + "authenticate-user-info");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json",
                "{\n  \"businessId\" : \"" + _uidBusinessId + "\",\n \"businessToken\" : \"" + _uidBusinessToken + "\"\n,\n \"nationalId\" : \"" + model.NationalId + "\"\n,\n \"nationalIdSerial\" : \"" + model.NationalIdSerial + "\"\n," +
                "\n \"gender\" : \"" + model.Gender + "\"\n,\n \"birthDate\" : \"" + model.BirthDate + "\"\n,\n \"downloadLink\" : \"" + model.DownloadLink + "\"\n }\n },\n ",
                ParameterType.RequestBody);           
            var response = client.Execute(request);
            var result = JsonSerializer.Deserialize<AuthKycResultViewModel>(response.Content, JsonHelper.JsonCamelCaseOption());
            return result;
        }

    }
}
