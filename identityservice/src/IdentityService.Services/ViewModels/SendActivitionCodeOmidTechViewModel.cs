namespace IdentityService.Services.ViewModels
{
    public class SendActivitionCodeOmidTechViewModel : BaseViewModel
    {
        public string MobileNumber { get; set; }
        public string Scheduler { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
        public string ApiKeyInternal { get; set; }
        public string Message { get; set; }

    }
}