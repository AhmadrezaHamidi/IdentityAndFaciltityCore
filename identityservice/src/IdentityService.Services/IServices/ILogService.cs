namespace IdentityService.Services.IServices
{
    public interface ILogService : IBaseService
    {
        Task AddLog(RequestInfo userInfo, object body, string url, CancellationToken cancellationToken);  
    }
}