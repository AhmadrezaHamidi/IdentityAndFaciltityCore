namespace IdentityService.Services.ViewModels
{
    public class RoleViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PersianTitle { get; set; }
        public bool IsActive { get; set; }

    }
}
