namespace IdentityService.Infrastructure.Configuration;

public class PolicyEntityConfiguration : IEntityTypeConfiguration<PolicyEntity>
{
    public void Configure(EntityTypeBuilder<PolicyEntity> builder)
    {
        builder.ToTable("Policy");
    }
}