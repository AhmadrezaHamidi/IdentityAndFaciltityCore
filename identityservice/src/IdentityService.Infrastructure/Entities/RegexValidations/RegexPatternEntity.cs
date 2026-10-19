using System.ComponentModel.DataAnnotations;
using IdentityService.Domain.Entity.RegexValidations.Enums;

namespace IdentityService.Infrastructure.Entities.RegexValidations;

public class RegexPatternEntity
{
    public int Id { get; set; }
    [MaxLength(512)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(128)]
    public string Pattern { get; set; } = string.Empty;
    [MaxLength(512)]
    public string ErrorMessage { get; set; } = string.Empty;
    public ERegexPlatform Platform { get; set; } = ERegexPlatform.General;
    public ICollection<PropertyRegexEntity> PropertyRegexes { get; set; } = [];
}