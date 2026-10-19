namespace IdentityService.Domain.Models.User
{
    public class AddPersonStatusModel : BaseModel
    {
        public int PersonId { get; set; }
        public bool IsNewPerson { get; set; }
    }
}
