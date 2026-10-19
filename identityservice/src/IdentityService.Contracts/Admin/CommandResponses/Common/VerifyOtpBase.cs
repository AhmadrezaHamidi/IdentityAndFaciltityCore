namespace IdentityService.Contracts.Admin.CommandResponses.Common;

public record VerifyOtpBase
{
    public string Token { get; set; } = string.Empty;
}