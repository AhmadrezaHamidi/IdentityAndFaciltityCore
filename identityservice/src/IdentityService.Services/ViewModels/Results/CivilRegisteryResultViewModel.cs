namespace IdentityService.Services.ViewModels.Results
{
    public partial class CivilRegisteryResultViewModel : BaseViewModel
    {
        public ResponseContextViewModel responseContext { get; set; }
        public BasicInformationViewModel basicInformation { get; set; }
        public IdentificationInformationViewModel identificationInformation { get; set; }
        public RegistrationStatusViewModel registrationStatus { get; set; }
    }
}
