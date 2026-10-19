


namespace IdentityService.Domain.Entity;

public partial class UserBranchEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RegionId { get; set; }

    public string? BranchCode { get; set; }

    public int SupervisorType { get; set; }

    public DateTime CreationTime { get; set; }
}
