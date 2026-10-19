namespace IdentityService.Infrastructure.Entities.Identities;

public class ActiveTokenEntity
{
    public long Id { get; set; }
    public string Token { get; private set; }
    public string MobileNumber { get; private set; }
    public string DeviceInfo { get; private set; }
    public string Version { get; private set; }
    public string Ip { get; private set; }
    public long TimeStamp { get; private set; }
    public bool IsActive { get; private set; }
}