namespace IdentityService.Contracts.RegexValidation.ReadModels.RetrieveRegexValidation;

public record RegexInformationDto(
    string Title,
    string Regex,
    string ErrorMessage
    );