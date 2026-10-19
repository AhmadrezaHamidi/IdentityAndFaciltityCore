namespace IdentityService.Services.ViewModels.V2.UpdateProfile.Request
{
    public class UpdateProfileRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string NationalCode { get; set; }
        public string Address { get; set; }
        public string InviteCode { get; set; }
        public int? CityId { get; set; }
        public long? Birthday { get; set; }
        public bool? Gender { get; set; }
    }
}
