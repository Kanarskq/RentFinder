using Bookings.Domain.AggregatesModel.BookingAggregate;

namespace Bookings.Api.Application.Queries.Bookings;

public interface IBookingQueries
{
    Task<Booking> GetBookingByIdAsync(int bookingId);
    Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId);
}
