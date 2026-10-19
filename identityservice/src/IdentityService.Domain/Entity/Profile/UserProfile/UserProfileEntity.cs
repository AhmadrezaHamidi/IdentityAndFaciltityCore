using System.ComponentModel.DataAnnotations;
using XAct.Diagnostics.Services.Implementations;

namespace IdentityService.Domain.Entity.Profile.UserPorfile;

public class UserProfileEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
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
    public string? JobTypeId { get; set; }
    public string? ReligionId { get; set; }
    public string? BirthCertificateNo { get; set; }
    public string? BirthCertificateSerialNo { get; set; }
    public string? IdentityCardNumber { get; set; }
    public string? IdentityCardSeries { get; set; }
    public string? IdentityCardSerial { get; set; }
    public DateTime? InquiryLastDate { get; set; }
    public bool? DeathStatus { get; set; }
    public bool? SejamStatus { get; set; }
    public virtual UserEntity User { get; set; } = null!;
    public string? RealFirstNameEn { get; set; }
    public string? RealLastNameEn { get; set; }
    public string? FatherNameEn { get; set; }
    public string? CityCode { get; set; }
    public string? StateCode { get; set; }
    public string? BirthCityCode { get; set; }
    public string? BirthStateCode { get; set; }
    public string? RegisterCityCode { get; set; }
    public string? RegisterStateCode { get; set; }
    public string? BranchCityCode { get; set; }
    public string? BranchStateCode { get; set; }
}