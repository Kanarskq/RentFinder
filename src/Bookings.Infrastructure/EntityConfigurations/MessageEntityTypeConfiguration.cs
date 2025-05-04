using Bookings.Domain.AggregatesModel.MessageAggregate;
using Bookings.Domain.AggregatesModel.PropertyAggregate;
using Bookings.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.Infrastructure.EntityConfigurations;

public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.SenderId)
            .IsRequired();

        builder.Property(b => b.ReceiverId)
            .IsRequired();

        builder.Property(b => b.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.SentAt)
            .IsRequired();

        builder.Property(b => b.ReadAt)
            .IsRequired(false);

        builder.Property(b => b.PropertyId)
            .IsRequired();

        builder.HasOne<Property>()
           .WithMany()
           .HasForeignKey(r => r.PropertyId)
           .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.SenderId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.ReceiverId);

        builder
            .HasIndex(m => new { m.SenderId, m.ReceiverId });

    }
}
