using IdentityService.Contracts.RegexValidation.ReadModels.RetrieveRegexValidation;

namespace IdentityService.Application.Handler.RegexValidations.CQS.Queries.RetrieveRegexValidation;

public record RetrieveRegexValidationQuery(string PropertyName) : BaseCommand<BaseMessageResponse<RetrieveRegexValidationDto>>;