namespace IdentityService.Services.ViewModels
{
    public class VerifyOtpForgotPasswordViewModel : BaseViewModel
    {
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string VerifyCode { get; set; }
        public Enums.OtpDeliveryTypeEnum DeliveryType { get; set; }
    }
}
