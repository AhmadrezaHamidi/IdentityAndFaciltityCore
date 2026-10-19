using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;

public record VinaraRetrieveUserInformationResponse
{
    [JsonProperty("base64_image")]
    [JsonPropertyName("base64_image")]
    public string Base64Image { get; init; } = string.Empty;
    
    [JsonProperty("national_code")]
    [JsonPropertyName("national_code")]
    public string NationalCode { get; init; } = string.Empty;
    
    [JsonProperty("gender")]
    [JsonPropertyName("gender")]
    public string Gender { get; init; } = string.Empty;
    
    [JsonProperty("phone")]
    [JsonPropertyName("phone")]
    public string PhoneNumber { get; init; } = string.Empty;
}