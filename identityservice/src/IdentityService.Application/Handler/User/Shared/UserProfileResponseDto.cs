namespace IdentityService.Application.Handler.User.Shared;

public class UserProfileResponseDto
{
    public string? RealFirstName { get; set; }
    public string? RealLastName { get; set; }
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string? CustomerGroupId { get; set; }
    public string? EducationId { get; set; }
    public string? ResidentialStatusId { get; set; }
    public string? SurnameId { get; set; }
    public string? SexId { get; set; }
    public string? RegisterLocationId { get; set; }
    public string? BirthLocationId { get; set; }
    public string? CountryId { get; set; }
    public string? BirthCountryId { get; set; }
    public string? MarriageId { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? BirthCertificateNo { get; set; }
    public string? BirthCertificateSerialNo { get; set; }
    public string? IdentityCardNumber { get; set; }
    public string? IdentityCardSeries { get; set; }
    public string? IdentityCardSerial { get; set; }
    public bool? DeathStatus { get; set; }
}