using OmidBank.Common.Models;

namespace IdentityService.Test.Common.Interface
{
    public interface IRequestHelper
    {
        Task<HttpResponseMessage> GetAsync(string requestUri, Dictionary<string, string>? additionalHeaders, CancellationToken cancellationToken, bool enableJwt = false);
        Task<HttpResponseMessage> PostAsync(string requestUri, object parameter, Dictionary<string, string>? additionalHeaders, CancellationToken cancellationToken,
        bool enableJwt = false, bool jsonCamelCaseOption = true);
        Task<HttpResponseMessage> SendEncryptedAsync(HttpMethod method, string requestUri, object parameter, Dictionary<string, string>? additionalHeaders, CancellationToken cancellationToken,
            bool enableJwt = false, bool jsonCamelCaseOption = true);
        Task<HttpResponseMessage> SendPreEncryptedAsync(HttpMethod method, string requestUri, EncryptionDataModel parameter, Dictionary<string, string>? additionalHeaders, CancellationToken cancellationToken,
            bool enableJwt = false, bool jsonCamelCaseOption = true);

    }
}