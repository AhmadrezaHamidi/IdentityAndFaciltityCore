namespace IdentityService.Services.ViewModels
{
    public class UpdateProfileViewModel : BaseViewModel
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string NationalCode { get; set; }
        public string AvatarName { get; set; }
        public string InvitedCode { get; set; }

        public string Address { get; set; }
        public int? CityId { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore]
        public DateTime? DateOfBirth { get; set; }
        public long? Birthday
        {
            get
            {
                if (DateOfBirth.HasValue) return DateOfBirth.Value.ToTimeStampSeconds();
                else return null;
            }
        }
        public bool? Gender { get; set; }
        public string CdnUrl { get; set; }
    }
}
