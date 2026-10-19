namespace IdentityService.Services.ViewModels
{
    public class SmsThirdPartyOmidTechRequestViewModel : BaseModel
    {
        public List<string> MobileNumbers { get; set; }
        public string MessageBody { get; set; }
        public string SmsType { get; set; }
        public string Scheduler { get; set; }
        public int TerminalType { get; set; }
    }
}
