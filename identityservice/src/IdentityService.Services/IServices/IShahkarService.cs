namespace IdentityService.Services.IServices
{
    public interface IShahkarService : IBaseService
    {
        Task<ShahkarResViewModel> ValidateIdentityByShahkar(ShahkarParameterViewModel model, RequestInfo clientInfo, CancellationToken cancellationToken);
    }
}