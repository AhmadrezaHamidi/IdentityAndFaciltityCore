namespace IdentityService.Services.ViewModels
{
    public class UserMessageViewModel : BaseViewModel
    {
        public int MessageId { get; set; }
        public List<string> UserMobiles { get; set; }
    }
}
