namespace IdentityService.Infrastructure.Configuration;

public class PersonOpenAccountEntityConfiguration : IEntityTypeConfiguration<PersonOpenAccountEntity>
{
    public void Configure(EntityTypeBuilder<PersonOpenAccountEntity> builder)
    {
        builder.ToTable("PersonOpenAccount");

        builder.HasIndex(e => e.PersonId, "IX_PersonOpenAccount_PersonId");

        builder.HasIndex(e => e.UniqueId, "IX_PersonOpenAccount_UniqueId");

        builder.HasIndex(e => e.UniqueId, "UX_PersonOpenAccount_UniqueId").IsUnique();

        builder.Property(e => e.ExpiryTime)
            .HasDefaultValueSql("(dateadd(minute,(1),getdate()))")
            .HasColumnType("datetime");
        builder.Property(e => e.UniqueId).HasDefaultValueSql("(newid())");

        builder.HasOne(d => d.Person).WithMany(p => p.PersonOpenAccounts)
            .HasForeignKey(d => d.PersonId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PersonOpenAccount_Person");
    }
}