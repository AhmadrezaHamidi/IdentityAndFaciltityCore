namespace IdentityService.Services.IServices
{
    public interface IPannelAuthService : IBaseService
    {
        Task<Tuple<bool, string, long, string, int>> PanelAuthenticate(string token, string controllerName, string actionName, string url, CancellationToken cancellationToken);
        Task<Tuple<bool, string, long, string, int>> PanelAuthenticateForToken(string token, string url, CancellationToken cancellationToken);
    }
}