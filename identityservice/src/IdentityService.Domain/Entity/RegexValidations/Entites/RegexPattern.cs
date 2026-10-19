using IdentityService.Domain.Entity.RegexValidations.Enums;

namespace IdentityService.Domain.Entity.RegexValidations.Entites;

public class RegexPattern
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Pattern { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public ERegexPlatform Platform { get; set; } = ERegexPlatform.General;
}