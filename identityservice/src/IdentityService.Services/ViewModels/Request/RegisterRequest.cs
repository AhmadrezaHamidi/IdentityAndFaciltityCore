namespace IdentityService.Services.ViewModels.Request
{
    public class RegisterRequest
    {
        public string MobileNumber { get; set; }
        public string NationalCode { get; set; }
        public string InvitedCode { get; set; }
    }
}