using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Bookings.Domain.AggregatesModel.BookingAggregate;

namespace Bookings.Infrastructure.EntityConfigurations;

public class BookingEntityTypeConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.PropertyId)
            .IsRequired();

        builder.Property(b => b.UserId)
            .IsRequired();

        builder.Property(b => b.StartDate)
            .IsRequired();

        builder.Property(b => b.EndDate)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasMaxLength(50);
    }
}
