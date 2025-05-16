using Bookings.Domain.AggregatesModel.BookingAggregate;
using Bookings.Domain.AggregatesModel.MessageAggregate;
using Bookings.Domain.AggregatesModel.PaymentAggregate;
using Bookings.Domain.AggregatesModel.PropertyAggregate;
using Bookings.Domain.AggregatesModel.ReviewAggregate;
using Bookings.Domain.AggregatesModel.UserAggregate;
using Bookings.Domain.SeedWork;
using Bookings.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Infrastructure;

public class BookingContext(DbContextOptions<BookingContext> options) : DbContext(options), IUnitOfWork
{
    internal const string RentFinderDbMigrationsHistoryTable = "__RentFinderDbMigrationsHistory";
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BookingEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyImageEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MessageEntityTypeConfiguration());
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}
