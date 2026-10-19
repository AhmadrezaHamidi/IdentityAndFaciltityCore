namespace IdentityService.Services.ViewModels.Request
{
	public class UserAuthenticationRequest
	{
        public string MobileNumber { get; set; }
        public string ActiveToken { get; set; }
        public bool? UserWithNewMobileNumber { get; set; } = false;
	}
}