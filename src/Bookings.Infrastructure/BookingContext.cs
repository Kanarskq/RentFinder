using Bookings.Domain.AggregatesModel.BookingAggregate;
using Bookings.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Infrastructure;

public class BookingContext : DbContext, IUnitOfWork
{
    public DbSet<Booking> Bookings { get; set; }

    public BookingContext(DbContextOptions<BookingContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingContext).Assembly);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}
