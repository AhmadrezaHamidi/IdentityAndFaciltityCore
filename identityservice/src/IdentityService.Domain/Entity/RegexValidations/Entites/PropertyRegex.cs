using IdentityService.Domain.Entity.RegexValidations.Enums;

namespace IdentityService.Domain.Entity.RegexValidations.Entites;

public class PropertyRegex
{
    public int RegexId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public int ValidationOrder { get; set; }
    public ERegexPlatform Platform { get; set; } = ERegexPlatform.General;
    public List<int> Regexes { get; set; } = [];
}