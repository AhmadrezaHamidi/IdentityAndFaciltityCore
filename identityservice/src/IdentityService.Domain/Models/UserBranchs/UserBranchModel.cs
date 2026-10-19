namespace IdentityService.Domain.Models.UserBranchs
{
    public class UserBranchModel : BaseModel
    {
        public int UserId { get; set; }
        public int RegionId { get; set; }
        public string? BranchCode { get; set; }
        public Enums.SupervisorType? SupervisorType { get; set; }
    }
}
