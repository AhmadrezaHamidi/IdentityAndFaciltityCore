namespace IdentityService.Domain.Models.User
{
    public class ReadUserMessagesModel : BaseModel
    {
        public int MessageId { get; set; }
        public bool Read { get; set; }
    }
}
