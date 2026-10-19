using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Configuration;

public class UserServiceAccessEntityConfiguration : IEntityTypeConfiguration<UserServiceAccessEntity>
{
    public void Configure(EntityTypeBuilder<UserServiceAccessEntity> builder)
    {
        builder.ToTable("UserServiceAccess");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ServiceKey)
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(e => e.UpdatedAtUtc).HasColumnType("datetime2");

        // یک کاربر برای هر خدمت فقط یک رکورد وضعیت دارد.
        builder.HasIndex(e => new { e.UserId, e.ServiceKey }, "UX_UserServiceAccess_UserId_ServiceKey").IsUnique();

        builder.HasOne(d => d.User).WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserServiceAccess_User");
    }
}
