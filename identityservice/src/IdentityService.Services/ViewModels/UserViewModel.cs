namespace IdentityService.Services.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
        public string NationalCode { get; set; }
        public string Password { get; set; }
        public float? Lat { get; set; }
        public float? Long { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string SelectedRoles { get; set; }

        //feature add region and branch for user
        public int? RegionId { get; set; }
        public string? BranchCode { get; set; }
        public Enums.SupervisorType? SupervisorType { get; set; }
        public bool IsBankerUser { get; set; }

    }
}
