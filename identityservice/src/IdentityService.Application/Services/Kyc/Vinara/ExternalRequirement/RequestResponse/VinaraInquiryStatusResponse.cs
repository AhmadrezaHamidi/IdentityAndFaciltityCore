using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;

public record VinaraInquiryStatusResponse(bool Success, string Timestamp, int Code, bool Finished, string Message, VinaraError? Error, 
    [property: JsonProperty("sync_status")] [property: JsonPropertyName("sync_status")] string SyncStatus);