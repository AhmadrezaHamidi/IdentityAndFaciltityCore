namespace IdentityService.Domain.Entity.ChangeMobileNumber;

public class ChangeMobileNumberOptions
{
    public string Agreements { get; set; }
    public ChangeMobileNumberIntroduction Introduction { get; set; }
    public bool EnableWage { get; set; }
    public long Wage { get; set; }
}