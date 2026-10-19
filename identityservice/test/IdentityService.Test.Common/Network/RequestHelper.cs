using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using IdentityService.Test.Common.Cryptography;
using OmidBank.Common.Helper;
using OmidBank.Common.Models;
using Interface_IRequestHelper = IdentityService.Test.Common.Interface.IRequestHelper;

namespace IdentityService.Test.Common.Network
{
    public class RequestHelper(HttpClient httpClient) : Interface_IRequestHelper
    {
        public async Task<HttpResponseMessage> GetAsync(string requestUri, Dictionary<string, string>? additionalHeaders, CancellationToken cancellationToken, bool enableJwt = false)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            AddHeaders(request, additionalHeaders, enableJwt);
            var responseMessage = await httpClient.SendAsync(request, cancellationToken);
            return responseMessage;
        }

        public static async Task<TResponse> ReadContentTask<TResponse>(HttpResponseMessage responseMessage, CancellationToken cancellationToken)
        {
            var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            var unescapedContent = JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                
            })!;
            return unescapedContent;
        }

        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string requestUri, object data,
            Dictionary<string, string>? additionalHeaders, CancellationToken cancellationToken,
            bool enableJwt = false, bool jsonCamelCaseOption = true)
        {
            var request = new HttpRequestMessage(method, requestUri);
            AddHeaders(request, additionalHeaders, enableJwt);
            var contentString = JsonSerializer.Serialize(data, jsonCamelCaseOption ? JsonHelper.JsonCamelCaseOption() : null);
            request.Content = new StringContent(contentString, Encoding.UTF8, "application/json");
            var responseMessage = await httpClient.SendAsync(request, cancellationToken);
            return responseMessage;
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, object data,
            Dictionary<string, string>? additionalHeaders, CancellationToken cancellationToken,
            bool enableJwt = false, bool jsonCamelCaseOption = true)
        {
            return await SendAsync(HttpMethod.Post, requestUri, data, additionalHeaders, cancellationToken, enableJwt, jsonCamelCaseOption);
        }

        public async Task<HttpResponseMessage> SendEncryptedAsync(HttpMethod method, string requestUri, object data,
            Dictionary<string, string>? additionalHeaders, CancellationToken cancellationToken,
            bool enableJwt = false, bool jsonCamelCaseOption = true)
        {
            var aesKey = AuthenticatedEncryptionService.GenerateKey();
            var service = new AuthenticatedEncryptionService(aesKey);
            var encryptedAesKey = Convert.ToBase64String(Encryption.EncryptRsa(Encoding.UTF8.GetBytes(aesKey)));
            var plainText = JsonSerializer.Serialize(data, jsonCamelCaseOption ? JsonHelper.JsonCamelCaseOption() : null);
            var encryptedData = service.Encrypt(plainText);
            var sendModel = new
            {
                EncryptedKey = encryptedAesKey,
                EncryptedData = encryptedData
            };
            
            var responseMessage = await SendAsync(method, requestUri, sendModel, additionalHeaders, cancellationToken, enableJwt, jsonCamelCaseOption);

            var encryptedText = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            var unescapedEncryptedText = JsonSerializer.Deserialize<string>(encryptedText)!;
            var decryptedText = service.Decrypt(unescapedEncryptedText);
            decryptedText = decryptedText.Replace("\r\n", "");
            responseMessage.Content = new StringContent(decryptedText, Encoding.UTF8, "application/json");
            return responseMessage;
        }

        public async Task<HttpResponseMessage> SendPreEncryptedAsync(HttpMethod method, string requestUri, EncryptionDataModel parameter, Dictionary<string, string>? additionalHeaders,
            CancellationToken cancellationToken, bool enableJwt = false, bool jsonCamelCaseOption = true)
        {
            var decryptedAesKey = Encryption.DecryptRsa(parameter.EncryptedKey);
            var service = new AuthenticatedEncryptionService(decryptedAesKey);
            var responseMessage = await SendAsync(method, requestUri, parameter, additionalHeaders, cancellationToken, enableJwt, jsonCamelCaseOption);

            var encryptedText = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            var unescapedEncryptedText = JsonSerializer.Deserialize<string>(encryptedText)!;
            var decryptedText = service.Decrypt(unescapedEncryptedText);
            decryptedText = decryptedText.Replace("\r\n", "");
            responseMessage.Content = new StringContent(decryptedText, Encoding.UTF8, "application/json");
            return responseMessage;
        }

        private static void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? additionalHeaders,
            bool enableJwt = false)
        {
            request.Headers.Add("Accept", "application/json");
            if (additionalHeaders is { Count: > 0 })
                foreach (var current in additionalHeaders)
                    request.Headers.Add(current.Key, current.Value);

            if (enableJwt)
            {
                //add bearer auth header
            }
        }
    }
}