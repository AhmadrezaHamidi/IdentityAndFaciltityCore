namespace IdentityService.Services.ViewModels
{
    public class CheckLastValidateByShahkarDateViewModel : BaseViewModel
    {
        public string MobileNumber { get; set; }
        /// <summary>
        /// تعداد ماه مورد نظر برای کنترل فعال بودن توکن
        /// </summary>
        public int Month { get; set; }
    }
}