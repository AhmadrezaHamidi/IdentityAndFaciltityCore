namespace IdentityService.Services.ViewModels
{
    public class UserMessageListViewModel : BaseViewModel
    {
        public int MessageId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public long DateTime { get; set; }
        public bool Read { get; set; }
    }
}
