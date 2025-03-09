using RentFinder.Web.Mvc.Models;

namespace RentFinder.Web.Mvc.Services;

public interface IBookingService
{
    Task<BookingResponseViewModel> CreateBookingAsync(BookingCreateModel bookingModel);
    Task<IEnumerable<BookingViewModel>> GetUserBookingsAsync();
    Task<BookingDetailViewModel> GetBookingDetailAsync(int bookingId);
    Task<bool> CancelBookingAsync(int bookingId);
}
