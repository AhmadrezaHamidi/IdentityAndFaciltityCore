namespace IdentityService.Services.ViewModels.Results
{
    public class BasicInformationViewModel : BaseViewModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fatherName { get; set; }
        public string gender { get; set; }
        public bool? IsCustomerSepah { get; set; }
        public string CustomerNumber { get; set; }
    }
}
