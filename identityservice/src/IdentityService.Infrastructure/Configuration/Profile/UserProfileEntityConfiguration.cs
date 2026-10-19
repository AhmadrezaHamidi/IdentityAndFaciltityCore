using IdentityService.Domain.Entity.Profile.UserPorfile;

namespace IdentityService.Infrastructure.Configuration.Profile;

public class UserProfileEntityConfiguration: IEntityTypeConfiguration<UserProfileEntity>
{
    public void Configure(EntityTypeBuilder<UserProfileEntity> builder)
    {
        builder.ToTable("UserProfile","Profile");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.RealFirstName).HasMaxLength(50).IsRequired(false);
        builder.Property(e => e.RealLastName).HasMaxLength(50).IsRequired(false);
        builder.Property(e => e.FatherName).HasMaxLength(50).IsRequired(false);
        builder.Property(e => e.MotherName).HasMaxLength(50).IsRequired(false);
        builder.Property(e => e.CustomerGroupId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.EducationId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.ResidentialStatusId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.SurnameId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.SexId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.RegisterLocationId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.BirthLocationId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.CountryId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.BirthCountryId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.MarriageId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.JobTypeId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.ReligionId).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.BirthCertificateNo).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.BirthCertificateSerialNo).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.IdentityCardNumber).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.IdentityCardSeries).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.IdentityCardSerial).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.DeathStatus).IsRequired(false);
        builder.Property(e => e.RealFirstNameEn).HasMaxLength(128).IsRequired(false).IsUnicode(false);
        builder.Property(e => e.RealLastNameEn).HasMaxLength(128).IsRequired(false).IsUnicode(false);
        builder.Property(e => e.FatherNameEn).HasMaxLength(128).IsRequired(false).IsUnicode(false);
        builder.Property(e => e.CityCode).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.StateCode).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.BirthCityCode).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.BirthStateCode).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.RegisterCityCode).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.RegisterStateCode).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.BranchCityCode).HasMaxLength(20).IsRequired(false);
        builder.Property(e => e.BranchStateCode).HasMaxLength(20).IsRequired(false);



        builder.Property(e => e.InquiryLastDate).HasColumnType("datetime").IsRequired(false);
        builder.HasOne(d => d.User).WithMany(p => p.UserProfiles)
            .HasForeignKey(d => d.UserId);
    }
}