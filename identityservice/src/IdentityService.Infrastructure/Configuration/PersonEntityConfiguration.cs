namespace IdentityService.Infrastructure.Configuration;

public class PersonEntityConfiguration : IEntityTypeConfiguration<PersonEntity>
{
    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        builder.ToTable("Person");

        builder.HasIndex(e => e.MobileNumber, "UX_Person_MobileNumber").IsUnique();

        builder.HasIndex(e => new { e.NationalCode, e.MobileNumber }, "UX_Person_NationalCode_MobileNumber").IsUnique();

        builder.Property(e => e.CreationTime).HasColumnType("datetime");
        builder.Property(e => e.BirthDate).HasColumnType("datetime").IsRequired(false);
        builder.Property(e => e.FirstName).HasMaxLength(50);
        builder.Property(e => e.LastName).HasMaxLength(50);
        builder.Property(e => e.NationalCardSeries).HasMaxLength(10);
        builder.Property(e => e.MobileNumber)
            .HasMaxLength(11)
            .IsUnicode(false);
        builder.Property(e => e.NationalCode)
            .HasMaxLength(11)
            .IsUnicode(false);
    }
}