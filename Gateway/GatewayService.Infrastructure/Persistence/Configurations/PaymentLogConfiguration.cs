using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GatewayService.Domain.Entities;

namespace GatewayService.Infrastructure.Persistence.Configurations;

public class PaymentLogConfiguration : BaseConfiguration<PaymentLog>
{
    public override void Configure(EntityTypeBuilder<PaymentLog> builder)
    {
        base.Configure(builder);

        builder.Property(t => t.Amount).HasPrecision(15, 0).IsRequired();
        builder.Property(t => t.Token).HasMaxLength(36).IsRequired();
        builder.Property(t => t.RRN).HasMaxLength(12).IsRequired(false);



    }
}