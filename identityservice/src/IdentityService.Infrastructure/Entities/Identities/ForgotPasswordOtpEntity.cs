namespace IdentityService.Infrastructure.Entities.Identities;

public class ForgotPasswordOtpEntity
{
    public long Id { get; set; }
    public string UserName { get; private set; }
    public string OtpCode { get; private set; }
    public string ReferenceId { get; private set; }
    public long TimeStamp { get; private set; }
    public byte DeliveryType { get; private set; }
    public string DeliveryAddress { get; private set; }
}