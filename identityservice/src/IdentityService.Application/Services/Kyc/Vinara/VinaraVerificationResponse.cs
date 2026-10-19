namespace IdentityService.Application.Services.Kyc.Vinara;

public record VinaraVerificationResponse(bool Success, DateTime Timestamp, string RedirectLink);