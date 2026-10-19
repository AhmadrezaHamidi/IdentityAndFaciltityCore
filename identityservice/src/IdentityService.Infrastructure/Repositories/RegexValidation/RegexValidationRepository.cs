using IdentityService.Contracts.RegexValidation.ReadModels.RetrieveRegexValidation;
using IdentityService.Contracts.RegexValidation.Repositories;
using IdentityService.Domain.Entity.RegexValidations.Enums;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.RegexValidation;

public class RegexValidationRepository(ApplicationDBContext dbContext) : IRegexValidationRepository
{
    public async Task<RetrieveRegexValidationDto> RetrieveRegexValidationAsync(string propertyName, ERegexPlatform platform, CancellationToken cancellationToken)
    {
        var regexPatterns = await dbContext.PropertyRegexs
            .Where(r => r.PropertyName == propertyName && (r.Platform == platform || r.Platform == ERegexPlatform.General))
            .OrderBy(p => p.ValidationOrder)
            .Select(r => new RegexInformationDto(
                r.RegexPattern.Title,
                r.RegexPattern.Pattern,
                r.RegexPattern.ErrorMessage
            ))
            .ToListAsync(cancellationToken: cancellationToken);
        return new RetrieveRegexValidationDto(regexPatterns);
    }
}