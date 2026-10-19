namespace IdentityService.Domain.Entity.UserManagementKycLog.Dto;

public class ReportUmDto
{
    public string NationalCode { get; set; }
    public string Mobile { get; set; }
    public string BirthDate { get; set; }
  //  public string Username { get; set; }
    public string RequestType { get; set; }
    public string Status { get; set; }
    public string NationalCardSeries { get; set; }

}