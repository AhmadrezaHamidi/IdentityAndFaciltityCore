namespace IdentityService.Application.Handler.User.Shared;

public class UserAddressResponseDto
{
    public string ?PostalCode { get; set; }
    public string? ProvinceName { get; set; }
    public string? CityName { get; set; }
    public string? TownShipName { get; set; }
    public string? ZoneName { get; set; }
    public string? VillageName { get; set; }
    public string? SubLocalityName { get; set; }
    public string? Street { get; set; }
    public float? HouseNumber { get; set; }
    public string? Floor { get; set; }
    public string? SideFloor { get; set; }
    public string? BuildingName { get; set; }
    public string? Description { get; set; }
}