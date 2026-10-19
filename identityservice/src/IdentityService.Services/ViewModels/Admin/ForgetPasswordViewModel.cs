namespace IdentityService.Services.ViewModels.Admin
{
    public class ForgetPasswordViewModel : BaseViewModel
    {
        public int Count { get; set; }
        public int Offset { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}