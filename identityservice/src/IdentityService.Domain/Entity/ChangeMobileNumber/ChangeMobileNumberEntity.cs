using OmidBank.Common.Base.Model;

namespace IdentityService.Domain.Entity.ChangeMobileNumber;

public class ChangeMobileNumberEntity : BaseEntity<int>
{
    public int UserId { get; set; }
    public string OldMobileNumber { get; set; }
    public string NewMobileNumber { get; set; }
    public string NationalId { get; set; }
    public string ReferenceCode { get; set; }
    public string TrackId { get; set; }
    public long WageAmount { get; set; }
    public bool IsLoggedIn { get; set; }
    public EChangeMobileNumberStatus Status { get; set; }
}