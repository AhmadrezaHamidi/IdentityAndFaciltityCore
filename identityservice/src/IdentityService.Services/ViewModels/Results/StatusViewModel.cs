namespace IdentityService.Services.ViewModels.Results
{
    public class StatusViewModel : BaseViewModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<object> details { get; set; }
    }
}
