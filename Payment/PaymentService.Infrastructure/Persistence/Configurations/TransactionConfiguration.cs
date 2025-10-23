using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : BaseConfiguration<Transaction>
{
    public override void Configure(EntityTypeBuilder<Transaction> builder)
    {
        base.Configure(builder);

        builder.Property(t => t.TerminalNo).HasMaxLength(10).IsRequired();
        builder.Property(t => t.Amount).HasPrecision(15,0).IsRequired();
        builder.Property(t => t.RedirectUrl).HasMaxLength(250).IsRequired();
        builder.Property(t => t.ReservationNumber).HasMaxLength(50).IsRequired();
        builder.Property(t => t.PhoneNumber).HasMaxLength(11).IsRequired();
        builder.Property(t => t.Token).HasMaxLength(36).IsRequired();
        builder.Property(t => t.RRN).HasMaxLength(12).IsRequired(false);
        builder.Property(t => t.Status).HasConversion<byte>() 
            .HasComment("وضعیت تراکنش: 0=Pending, 1=Success, 2=Failed, 3=Expired");
        builder.Property(t => t.AppCode).HasMaxLength(50).IsRequired(false);


    }
}