namespace IdentityService.Services.ViewModels
{
    public class ReqTryCountViewModel : BaseViewModel
    {
        public string Ip { get; set; }
        public string MobileNumber { get; set; }
        public string Url { get; set; }
        public int ServiceStatus { get; set; }
        public Enums.ServiceTypeTryCount? ServiceType { get; set; }
    }
}
