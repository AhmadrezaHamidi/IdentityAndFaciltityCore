namespace IdentityService.Infrastructure.Configuration;

public class PersonDetailEntityConfiguration : IEntityTypeConfiguration<PersonDetailEntity>
{
    public void Configure(EntityTypeBuilder<PersonDetailEntity> builder)
    {
        builder.ToTable("PersonDetail");

        builder.HasIndex(e => e.CityId, "IX_PersonDetail_CityId");

        builder.HasIndex(e => e.PersonId, "IX_PersonDetail_PersonId");

        builder.Property(e => e.Address).HasMaxLength(250);
        builder.Property(e => e.AvatarName)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.DateOfBirth).HasColumnType("datetime");
        builder.Property(e => e.ModifiedDate).HasColumnType("datetime");

        builder.HasOne(d => d.Person).WithMany(p => p.PersonDetails)
            .HasForeignKey(d => d.PersonId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PersonDetail_Person");
    }
}