using IdentityService.Domain.IRepositories;

namespace IdentityService.Services.Services;

public class ActiveTokenService : BaseService, IActiveTokenService
{

    private readonly ILogger<ActiveTokenService> _logger;
    private readonly IActiveTokenRepository _activeTokenRepository;

    public ActiveTokenService(ILogger<ActiveTokenService> logger, IActiveTokenRepository activeTokenRepository)
    {
        _logger = logger;
        _activeTokenRepository = activeTokenRepository;
    }

    public async Task<bool> DeActiveTokenAsync(string mobileNumber, string token, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mobileNumber) || string.IsNullOrWhiteSpace(token)) return false;

            await _activeTokenRepository.DeActiveTokenAsync(mobileNumber, token, cancellationToken);
            return true;
        }
        catch (Exception exp)
        {
            _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
            return false;
        }
    }
}
