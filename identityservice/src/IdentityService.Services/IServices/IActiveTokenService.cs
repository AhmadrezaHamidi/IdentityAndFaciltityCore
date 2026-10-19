namespace IdentityService.Services.IServices;

public interface IActiveTokenService : IBaseService
{
    Task<bool> DeActiveTokenAsync(string mobileNumber, string token, CancellationToken cancellationToken);
}
