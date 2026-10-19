namespace IdentityService.Domain.Models.Sms
{
    public class SmsThirdPartyRequestModel : BaseModel
    {
        public List<string> MobileNumbers { get; set; }
        public string MessageBody { get; set; }
        public int AllowDelay { get; set; }
    }
}
