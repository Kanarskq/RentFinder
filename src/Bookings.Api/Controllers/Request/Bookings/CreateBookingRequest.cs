namespace Bookings.Api.Controllers.Request.Bookings;

public record CreateBookingRequest(
    int BookingId,
    int PropertyId,
    int UserId,
    DateTime StartDate,
    DateTime EndDate,
    DateTime CreatedAt,
    string Status,
    decimal TotalPrice
);
