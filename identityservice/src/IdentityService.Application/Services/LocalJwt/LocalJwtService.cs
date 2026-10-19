using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.Contracts.Services.JwtService;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Application.Services.LocalJwt;

public class LocalJwtService(string secretKey, string issuer, string audience) : IJwtService
{
    public string GenerateToken(IEnumerable<Claim> claims, TimeSpan? expiry = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(expiry ?? TimeSpan.FromMinutes(5)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateToken(Dictionary<string, object> data, TimeSpan? expiry = null)
    {
        var claims = DictionaryToClaims(data);
        return GenerateToken(claims, expiry);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, parameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
    
    public List<Claim> DictionaryToClaims(Dictionary<string, object> data)
    {
        return data.Select(item => new Claim(item.Key, item.Value.ToString()!)).ToList();
    }
}