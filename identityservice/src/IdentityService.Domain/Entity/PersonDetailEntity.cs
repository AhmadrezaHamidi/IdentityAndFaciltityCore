


namespace IdentityService.Domain.Entity;

public partial class PersonDetailEntity
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public int? CityId { get; set; }

    public bool? Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Address { get; set; }

    public string? AvatarName { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual PersonEntity Person { get; set; } = null!;
}
