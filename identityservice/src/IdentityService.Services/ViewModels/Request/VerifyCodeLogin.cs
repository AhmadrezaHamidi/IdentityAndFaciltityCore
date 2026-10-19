namespace IdentityService.Services.ViewModels.Request
{
    public class VerifyCodeLogin
    {
        public string FirebaseKey { get; set; }
        public string OtpCode { get; set; }
    }
}