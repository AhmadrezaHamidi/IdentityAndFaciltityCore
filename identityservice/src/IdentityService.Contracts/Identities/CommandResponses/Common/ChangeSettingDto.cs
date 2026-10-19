namespace IdentityService.Contracts.Identities.CommandResponses.Common;

public record ChangeSettingDto
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}