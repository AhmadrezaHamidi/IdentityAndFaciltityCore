using IdentityService.Contracts.RegexValidation.ReadModels.RetrieveRegexValidation;
using IdentityService.Domain.Entity.RegexValidations.Enums;

namespace IdentityService.Contracts.RegexValidation.Repositories;

public interface IRegexValidationRepository
{
    Task<RetrieveRegexValidationDto> RetrieveRegexValidationAsync(string propertyName, ERegexPlatform platform, CancellationToken cancellationToken);
}