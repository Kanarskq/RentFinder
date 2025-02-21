namespace Bookings.Api.Application.Queries;

public interface IBookingQueries
{
    Task<Booking> GetBookingByIdAsync(int bookingId);
    Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId);
}
