namespace IdentityService.Services.ViewModels
{
    public class ResSearchRealCustomerByNationalCodeViewModel : BaseViewModel
    {
        public int RsCode { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public ResultDataSearchRealCustomerByNationalCodeViewModel ResultData { get; set; }
    }
}
