namespace IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;

public class VinaraBaseResponse<TData>
{
    public TData? Result { get; set; }
    public string Timestamp { get; set; } = string.Empty;
    public bool Success { get; set; }
    public VinaraError? Error { get; set; }
}