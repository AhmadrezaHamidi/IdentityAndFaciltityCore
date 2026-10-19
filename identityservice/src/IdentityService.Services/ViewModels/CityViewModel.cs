namespace IdentityService.Services.ViewModels
{
    public class CityViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public string EnTitle { get; set; }
    }
    
    public record struct CityInformationDTO(int CityId, string CityTitle, string CityEnTitle, int ProvinceId, string ProvinceTitle, string ProvinceEnTitle);
}
