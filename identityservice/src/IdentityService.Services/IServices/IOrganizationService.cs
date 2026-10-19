namespace IdentityService.Services.IServices
{
    public interface IOrganizationService : IBaseService
    {
        Task<OrganizationModel> GetOrganizationAsync(string token, int organizationId, string url, CancellationToken cancellationToken);
    }
}