namespace IdentityService.Services.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        public string NationalCode { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public Enums.OtpDeliveryTypeEnum DeliveryType { get; set; }
    }
}
