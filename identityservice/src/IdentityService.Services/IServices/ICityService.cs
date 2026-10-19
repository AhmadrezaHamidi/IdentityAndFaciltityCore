using IdentityService.Services.ViewModels;

namespace IdentityService.Services.IServices
{
    public interface ICityService : IBaseService
    {
        Task<CityInformationDTO> GetCityById(int? cityId, CancellationToken cancellationToken); 
    }
}
