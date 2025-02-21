using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.BookingAggregate;

public class Booking : Entity, IAggregateRoot
{
    public int BookingId { get; private set; }
    public int PropertyId { get; private set; }
    public int UserId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Status { get; private set; }

    private Booking() { }

    public Booking(int propertyId, int userId, DateTime startDate, DateTime endDate)
    {
        PropertyId = propertyId;
        UserId = userId;
        StartDate = startDate;
        EndDate = endDate;
        CreatedAt = DateTime.UtcNow;
        Status = "Pending";
    }

    public void ConfirmBooking()
    {
        Status = "Confirmed";
    }

    public void CancelBooking()
    {
        Status = "Cancelled";
    }
}

