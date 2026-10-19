namespace IdentityService.Services.ViewModels.V2.Profile.Response
{
    public class ProfileDetailResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public int? CityId { get; set; }
        public string CityTitle { get; set; }
        public string CityEnTitle { get; set; }
        public string AvatarUrl { get; set; }

        public int? ProvinceId { get; set; }
        public string ProvinceTitle { get; set; }
        public string ProvinceEnTitle { get; set; }
        public string? InvitedCode { get; set; }
        public string? MyInviteCode { get; set; }
    }
}
