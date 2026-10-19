using OmidBank.Common.Base.Model;

namespace IdentityService.Domain.Entity.UserManagement.ChangeUserActiveStateHistory;

public class ChangeUserActiveStateHistory : BaseEntity<int>
{
    public bool IsSuccess { get; set; }
    public int UserId { get; set; }
}
