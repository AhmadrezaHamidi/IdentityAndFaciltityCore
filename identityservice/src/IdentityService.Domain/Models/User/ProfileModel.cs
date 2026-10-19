namespace IdentityService.Domain.Models.User
{
    public class ProfileModel : BaseModel
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string NationalCode { get; set; }
        public string Address { get; set; }
        public string AvatarName { get; set; }
        public int? CityId { get; set; }
        // public string CityTitle { get; set; }
        // public string CityEnTitle { get; set; }
        public bool IsActive { get; set; }
        public long? Birthday { get; set; }
        public bool? Gender { get; set; }
        public string CdnUrl { get; set; }
        public bool IsActiveProfile { get; set; }
        public string? InvitedCode { get; set; }
        public string? MyInviteCode { get; set; }
    }
}
