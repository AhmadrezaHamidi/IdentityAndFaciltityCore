namespace IdentityService.Infrastructure.Entities.Identities;

public class ActiveCodeEntity
{
    public long Id { get; set; }
    public string MobileNumber { get; private set; }
    public string Code { get; private set; }
    public long TimeStamp { get; private set; }
    public bool IsActive { get; set; }
    public string PersianDateTime { get; set; }
    public Enums.ActivationCode Type { get; set; } = Enums.ActivationCode.Register;
}