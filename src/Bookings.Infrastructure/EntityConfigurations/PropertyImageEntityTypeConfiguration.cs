using Bookings.Domain.AggregatesModel.PropertyAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Infrastructure.EntityConfigurations;

public class PropertyImageEntityTypeConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.PropertyId)
            .IsRequired();

        builder.Property(i => i.ImageData)
            .IsRequired()
            .HasColumnType("varbinary(max)"); 

        builder.Property(i => i.ImageType)
            .IsRequired()
            .HasMaxLength(50); 

        builder.Property(i => i.Caption)
            .HasMaxLength(200);

        builder.Property(i => i.UploadedAt)
            .IsRequired();
    }
}