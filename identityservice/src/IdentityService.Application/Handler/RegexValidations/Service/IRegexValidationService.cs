using IdentityService.Application.Handler.RegexValidations.CQS.Queries.RetrieveRegexValidation;
using IdentityService.Contracts.Identities.ReadModels;
using IdentityService.Contracts.RegexValidation.ReadModels.RetrieveRegexValidation;

namespace IdentityService.Application.Handler.RegexValidations.Service;

public interface IRegexValidationService
{
    Task<BaseMessageResponse<RetrieveRegexValidationDto>> RetrieveRegexValidation(RetrieveRegexValidationQuery request, CancellationToken cancellationToken);
    Task<ValidatePropertyDto> ValidatePropertyAsync(string property, string value, CancellationToken cancellationToken);
}