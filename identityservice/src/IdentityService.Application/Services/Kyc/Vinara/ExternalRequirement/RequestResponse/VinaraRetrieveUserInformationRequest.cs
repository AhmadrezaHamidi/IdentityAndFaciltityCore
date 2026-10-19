namespace IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;

public record VinaraRetrieveUserInformationRequest(string UniqueKey) : BaseCommand<VinaraBaseResponse<VinaraRetrieveUserInformationResponse>>;