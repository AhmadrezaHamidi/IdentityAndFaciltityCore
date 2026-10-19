using IdentityService.Services.ViewModels;

namespace IdentityService.Services.IServices
{
    public interface ICivilRegistryServices : IBaseService
    {
        CivilRegisteryResultViewModel PersonValidateByCivilRegistery(CivilRegisteryParameterViewModel model, CancellationToken cancellationToken);
        Task<GetSearchRealCustomerViewModel> SearchRealCustomerByNationalCode(string nationalCode, string url, string apiKeyApiBankSepah, CancellationToken cancellationToken);
    }
}
