using System.Security.Claims;

namespace IdentityService.Contracts.Services.JwtService;

public interface IJwtService
{
    string GenerateToken(IEnumerable<Claim> claims, TimeSpan? expiry = null);
    string GenerateToken(Dictionary<string, object> data, TimeSpan? expiry = null);
    ClaimsPrincipal? ValidateToken(string token);
    List<Claim> DictionaryToClaims(Dictionary<string, object> data);

}