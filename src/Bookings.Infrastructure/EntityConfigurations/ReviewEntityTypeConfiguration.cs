using Bookings.Domain.AggregatesModel.ReviewAggregate;
using Bookings.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Bookings.Domain.AggregatesModel.PropertyAggregate;

namespace Bookings.Infrastructure.EntityConfigurations;

public class ReviewEntityTypeConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.PropertyId)
            .IsRequired();

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        builder.HasOne<Property>()
            .WithMany()
            .HasForeignKey(r => r.PropertyId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId);
    }
}
