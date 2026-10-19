using System.ComponentModel.DataAnnotations;
using IdentityService.Domain.Entity.RegexValidations.Enums;

namespace IdentityService.Infrastructure.Entities.RegexValidations;

public class PropertyRegexEntity
{
    [MaxLength(128)]
    public int RegexId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public int ValidationOrder { get; set; }
    public ERegexPlatform Platform { get; set; } = ERegexPlatform.General;
    public RegexPatternEntity RegexPattern { get; set; }
}