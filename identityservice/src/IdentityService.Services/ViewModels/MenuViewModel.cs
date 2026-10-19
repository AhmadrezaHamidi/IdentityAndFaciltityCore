namespace IdentityService.Services.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public string EnTitle { get; set; }
        public string Url { get; set; }
        public string IconName { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public bool IsRootMenu { get; set; }
        public bool IsInternalMenu { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
    }
}
