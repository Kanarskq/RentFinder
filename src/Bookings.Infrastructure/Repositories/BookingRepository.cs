using Bookings.Domain.AggregatesModel.BookingAggregate;
using Bookings.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bookings.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingContext _context;

    public BookingRepository(BookingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public Booking Add(Booking booking)
    {
        return _context.Bookings.Add(booking).Entity;
    }

    public async Task<Booking?> GetAsync(int bookingId)
    {
        return await _context.Bookings
            .AsNoTracking() 
            .FirstOrDefaultAsync(b => b.Id == bookingId);
    }

    public void Update(Booking booking)
    {
        _context.Bookings.Update(booking);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
