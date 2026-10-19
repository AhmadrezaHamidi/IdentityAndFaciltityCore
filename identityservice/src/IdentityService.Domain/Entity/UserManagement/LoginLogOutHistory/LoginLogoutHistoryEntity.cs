namespace IdentityService.Domain.Entity.UserManagement.LoginLogOutHistory;

public class LoginLogoutHistoryEntity
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public virtual UserEntity User { get; set; }

    public DateTime? CreateDate { get; set; }

    public int TypeAction { get; set; }

    public int Status { get; set; }

    public string Ip { get; set; }

    public string DeviceInfo { get; set; }
}