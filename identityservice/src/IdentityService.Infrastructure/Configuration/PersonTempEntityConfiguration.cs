using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Infrastructure.Configuration;

public class PersonTempEntityConfiguration : IEntityTypeConfiguration<PersonTempEntity>
{
    public void Configure(EntityTypeBuilder<PersonTempEntity> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_PersonTemp_Id");

        builder.ToTable("PersonTemp");

        builder.Property(e => e.ApplicationVersion).HasMaxLength(20);
        builder.Property(e => e.InvitedCode)
            .HasMaxLength(11)
            .IsUnicode(false);
        builder.Property(e => e.LastIpAddress).HasMaxLength(30);
        builder.Property(e => e.LastSentActivationCodeDateTime).HasColumnType("datetime");
        builder.Property(e => e.LastValidateByShahkarDateTime).HasColumnType("datetime");
        builder.Property(e => e.MobileNumber).HasMaxLength(25);
        builder.Property(e => e.NationalCode).HasMaxLength(15);
        builder.Property(e => e.RegisterDate)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.VerifiedActivationCodeDateTime).HasColumnType("datetime");
        builder.Property(e => e.VerifiedByShahkarDateTime).HasColumnType("datetime");

        builder.HasOne(d => d.Person).WithMany(p => p.PersonTemps)
            .HasForeignKey(d => d.PersonId)
            .HasConstraintName("FK_PersonTemp_Person");
    }
}