using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Loans;

public sealed class LoanRequestHistoryConfiguration : IEntityTypeConfiguration<LoanRequestHistory>
{
    public void Configure(EntityTypeBuilder<LoanRequestHistory> e)
    {
        e.ToTable("LoanRequestHistory", Schema);
        e.Property(x => x.Reason).HasMaxLength(200);
        e.Property(x => x.Description).HasMaxLength(1000);
        RegistrarColumn(e.Property(x => x.By));
    }
}
