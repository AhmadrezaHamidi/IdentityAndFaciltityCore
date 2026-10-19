namespace IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;

public class VinaraError
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
}