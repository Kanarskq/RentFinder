using Bookings.Domain.AggregatesModel.PaymentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.Infrastructure.EntityConfigurations;

public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> paymentConfiguration)
    {
        paymentConfiguration.ToTable("payments");

        paymentConfiguration.HasKey(p => p.Id);
        paymentConfiguration.Property(p => p.Id)
            .IsRequired();

        paymentConfiguration.Property(p => p.BookingId)
            .IsRequired();

        paymentConfiguration.Property(p => p.UserId)
            .IsRequired();

        paymentConfiguration.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        paymentConfiguration.Property(p => p.Currency)
            .HasMaxLength(3)
            .IsRequired();

        paymentConfiguration.Property(p => p.PaymentMethod)
            .HasMaxLength(50)
            .IsRequired();

        paymentConfiguration.Property(p => p.Status)
            .HasMaxLength(20)
            .IsRequired();

        paymentConfiguration.Property(p => p.TransactionId)
            .HasMaxLength(100);

        paymentConfiguration.Property(p => p.ProviderReference)
            .HasMaxLength(100);

        paymentConfiguration.Property(p => p.CreatedAt)
            .IsRequired();

        paymentConfiguration.Property(p => p.ProcessedAt);

        paymentConfiguration.Property(p => p.ErrorMessage)
            .HasMaxLength(500);

        paymentConfiguration.Property(p => p.PaymentIntent)
            .HasMaxLength(100);

        paymentConfiguration.HasIndex(p => p.BookingId);
        paymentConfiguration.HasIndex(p => p.UserId);
        paymentConfiguration.HasIndex(p => p.Status);
        paymentConfiguration.HasIndex(p => p.TransactionId).IsUnique();
    }
}