using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.BookingAggregate;

public interface IBookingRepository : IRepository<Booking>
{
    Booking Add(Booking booking);
    void Update(Booking booking);
    Task<Booking> GetAsync (int bookingid);
    Task<IEnumerable<Booking>> GetByPropertyAsync(int propertyId);
    Task<IEnumerable<Booking>> GetByUserAsync(int userId);
    Task<IEnumerable<Booking>> GetByStatusAsync(string status);

}
