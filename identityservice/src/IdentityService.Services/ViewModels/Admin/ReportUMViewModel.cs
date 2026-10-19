namespace IdentityService.Services.ViewModels.Admin;

public class ReportUMViewModel : BaseViewModel
{
    public string NationalCode { get; set; }
    public string Mobile { get; set; }
    public string BirthDateFrom { get; set; }
    public string BirthDateTo { get; set; }
    public string Username { get; set; }
    public string RequestType { get; set; }
    public string Status { get; set; }

    public string Description { get; set; }
    public int Offset { get; set; }
    public int Count { get; set; }
}