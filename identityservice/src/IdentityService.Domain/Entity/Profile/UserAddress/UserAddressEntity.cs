namespace IdentityService.Domain.Entity.Profile.UserAddress;

public class UserAddressEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ?PostalCode { get; set; }
    public string? ProvinceName { get; set; }
    public string? CityName { get; set; }
    public string? TownShipName { get; set; }
    public string? ZoneName { get; set; }
    public string? VillageName { get; set; }
    public string? SubLocalityName { get; set; }
    public string? Street { get; set; }
    public double? HouseNumber { get; set; }
    public string? Floor { get; set; }
    public string? Unit { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SideFloor { get; set; }
    public string? BuildingName { get; set; }
    public string? Description { get; set; }
    public DateTime? InquiryLastDate { get; set; }
    public virtual UserEntity User { get; set; } = null!;

}