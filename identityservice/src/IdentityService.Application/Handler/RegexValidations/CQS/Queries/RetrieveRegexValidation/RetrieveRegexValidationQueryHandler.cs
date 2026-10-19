using IdentityService.Application.Handler.RegexValidations.Service;
using IdentityService.Contracts.RegexValidation.ReadModels.RetrieveRegexValidation;

namespace IdentityService.Application.Handler.RegexValidations.CQS.Queries.RetrieveRegexValidation;

public class RetrieveRegexValidationQueryHandler(IRegexValidationService regexValidationService) : IBaseHandler<RetrieveRegexValidationQuery, BaseMessageResponse<RetrieveRegexValidationDto>>
{
    public async Task<BaseMessageResponse<RetrieveRegexValidationDto>> Handle(RetrieveRegexValidationQuery request, CancellationToken cancellationToken)
    {
        var result = await regexValidationService.RetrieveRegexValidation(request, cancellationToken);
        return result;
    }
}