



namespace IdentityService.Domain.Entity;


public partial class PersonEntity
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? NationalCode { get; set; }

    public string MobileNumber { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public string NationalCardSeries { get; set; } 

    public DateTime CreationTime { get; set; }

    public bool? IsVerified { get; set; }

    public virtual PanelUserEntity? PanelUser { get; set; }

    public virtual ICollection<PersonDetailEntity> PersonDetails { get; set; } = new List<PersonDetailEntity>();

    public virtual ICollection<PersonOpenAccountEntity> PersonOpenAccounts { get; set; } = new List<PersonOpenAccountEntity>();

    public virtual ICollection<PersonTempEntity> PersonTemps { get; set; } = new List<PersonTempEntity>();

    public virtual ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
}
