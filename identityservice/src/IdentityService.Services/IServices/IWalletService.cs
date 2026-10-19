namespace IdentityService.Services.IServices
{
    public interface IWalletService : IBaseService
    {
       Task<bool> AddWallet(UserModel user, string token, string url,string apikeyInternal, CancellationToken cancellationToken);
    }
}