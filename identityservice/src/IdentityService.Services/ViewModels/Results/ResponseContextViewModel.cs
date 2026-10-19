namespace IdentityService.Services.ViewModels.Results
{
    public class ResponseContextViewModel : BaseViewModel
    {
        public StatusViewModel status { get; set; }
        public string requestId { get; set; }
        public string correlationId { get; set; }
        public string navigationURI { get; set; }
        public string nextStepToken { get; set; }
        public string userSessionId { get; set; }
        public object custom { get; set; }
    }
}
