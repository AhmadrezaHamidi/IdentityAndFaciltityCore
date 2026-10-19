using System.Text.RegularExpressions;
using IdentityService.Application.Handler.RegexValidations.CQS.Queries.RetrieveRegexValidation;
using IdentityService.Contracts.Identities.ReadModels;
using IdentityService.Contracts.RegexValidation.ReadModels.RetrieveRegexValidation;
using IdentityService.Contracts.RegexValidation.Repositories;
using IdentityService.Domain.Entity.RegexValidations.Enums;

namespace IdentityService.Application.Handler.RegexValidations.Service;

public class RegexValidationService(IRegexValidationRepository regexValidationRepository) : IRegexValidationService
{
    public async Task<BaseMessageResponse<RetrieveRegexValidationDto>> RetrieveRegexValidation(RetrieveRegexValidationQuery request, CancellationToken cancellationToken)
    {
        var platform = ConvertAppIdToRegexPlatform(request.RequestInfo.AppId);
        var response = await regexValidationRepository.RetrieveRegexValidationAsync(request.PropertyName, platform, cancellationToken);
        var result = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, response);
        return result!;
    }

    public async Task<ValidatePropertyDto> ValidatePropertyAsync(string property, string value, CancellationToken cancellationToken)
    {
        var response = await regexValidationRepository.RetrieveRegexValidationAsync(property, ERegexPlatform.General, cancellationToken);
        foreach (var regex in response.Items.Where(regex => !Regex.IsMatch(value, regex.Regex)))
            return new ValidatePropertyDto(false, regex.ErrorMessage);
        
        return new ValidatePropertyDto(true, string.Empty);
    }

    private static ERegexPlatform ConvertAppIdToRegexPlatform(Enums.AppEnum appId)
    {
        return appId switch
        {
            Enums.AppEnum.MobileApp => ERegexPlatform.AndroidKotlin,
            Enums.AppEnum.PWA or Enums.AppEnum.OWP => ERegexPlatform.BrowserJavaScript,
            Enums.AppEnum.IOS or Enums.AppEnum.Swagger or Enums.AppEnum.AdminPannel or Enums.AppEnum.InternalService =>
                ERegexPlatform.General,
            _ => throw new ArgumentOutOfRangeException(nameof(appId), appId, null)
        };
    }
}