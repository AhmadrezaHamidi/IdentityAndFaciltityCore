namespace IdentityService.Domain.Models.User
{
    public class RoleModel : BaseModel
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string PersianTitle { get; private set; }
        public bool IsActive { get; private set; }

        public RoleModel()
        {

        }

        public RoleModel(int id, string title, string persianTitle, bool isActive)
        {
            Id = id;
            Title = title;
            PersianTitle = persianTitle;
            IsActive = isActive;
        }
    }
}
