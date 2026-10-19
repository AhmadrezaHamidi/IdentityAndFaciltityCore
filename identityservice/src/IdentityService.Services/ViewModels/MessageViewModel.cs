namespace IdentityService.Services.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int Type { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsNotify { get; set; }
        public bool IsInternal { get; set; }
        public bool IsSms { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
