using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Bookings.Domain.AggregatesModel.PropertyAggregate;

namespace Bookings.Infrastructure.EntityConfigurations;

public class PropertyEntityTypeConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.OwnerId)
            .IsRequired();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.Latitude)
            .IsRequired();

        builder.Property(p => p.Longitude)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.Price)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Bedrooms)
            .IsRequired();

        builder.Property(p => p.Bathrooms)
            .IsRequired();

        builder.Property(p => p.SquareFootage)
            .IsRequired();

        builder.Property(p => p.HasBalcony)
            .IsRequired();

        builder.Property(p => p.HasParking)
            .IsRequired();

        builder.Property(p => p.PetsAllowed)
            .IsRequired();

        builder.Property(p => p.PropertyType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.YearBuilt)
            .IsRequired();

        builder.HasMany(p => p.Images)
           .WithOne()
           .HasForeignKey(i => i.PropertyId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
