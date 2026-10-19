namespace IdentityService.Services.ViewModels.Request
{
    public class ResendOtpCodeRequest
    {
        public Enums.ActivationCode OtpCodeType { get; set; }
    }
}