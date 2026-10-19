


namespace IdentityService.Domain.Entity;

public partial class ChangePasswordUserManagmentLogEntity
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public string? LastPassword { get; set; }

    public string? NewPassword { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? Status { get; set; }

    public string? Ip { get; set; }

    public string? DeviceInfo { get; set; }

    public virtual UserEntity User { get; set; } = null!;
}
