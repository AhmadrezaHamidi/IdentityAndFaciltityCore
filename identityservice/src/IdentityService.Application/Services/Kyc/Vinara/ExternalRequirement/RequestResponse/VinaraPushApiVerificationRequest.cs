namespace IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;

public record VinaraPushApiVerificationRequest(string Token, string Status, VinaraError Error) : BaseCommand<VinaraBaseResponse<object>>;
