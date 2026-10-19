using IdentityService.Domain.Entity.Profile.UserAddress;

namespace IdentityService.Infrastructure.Configuration.Profile;

public class UserAddressEntityConfiguration : IEntityTypeConfiguration<UserAddressEntity>
{
    public void Configure(EntityTypeBuilder<UserAddressEntity> builder)
    {      
        builder.ToTable("UserAddress","Profile");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.PostalCode).HasMaxLength(11).IsRequired(false);
        builder.Property(e => e.ProvinceName).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.CityName).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.TownShipName).HasMaxLength(200).IsRequired(false);
        builder.Property(e => e.ZoneName).HasMaxLength(200).IsRequired(false);
        builder.Property(e => e.VillageName).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.SubLocalityName).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.Street).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.HouseNumber).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.Floor).IsRequired(false);
        builder.Property(e => e.SideFloor).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.BuildingName).HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.Description).IsRequired(false);
        builder.Property(e => e.PhoneNumber).HasMaxLength(15).IsRequired(false);
        builder.Property(e => e.Unit).HasMaxLength(50).IsRequired(false);
        builder.Property(e => e.InquiryLastDate).HasColumnType("datetime").IsRequired(false);
        
        builder.HasOne(d => d.User).WithMany(p => p.UserAddress).HasForeignKey(d => d.UserId).HasPrincipalKey(u => u.Id); ;
    }
}