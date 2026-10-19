namespace IdentityService.Domain.Models.User
{
    public class PanelMenuRoleModel : BaseModel
    {
        public long Id { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}
