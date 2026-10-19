namespace IdentityService.Contracts.Identities.CommandResponses.Common;

public record VerifyOtpBase
{
    public string Token { get; set; } = string.Empty;
}