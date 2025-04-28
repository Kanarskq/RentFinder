using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.BookingAggregate;

public class Booking : Entity, IAggregateRoot
{
    public int Id { get; private set; }
    public int PropertyId { get; private set; }
    public int UserId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Status { get; private set; }
    public decimal TotalPrice { get; private set; }
    public int? PaymentId { get; private set; }
    public string PaymentStatus { get; private set; } = "";
    public string? PaymentMethod { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime? PaymentDate { get; private set; }

    private Booking() { }

    public Booking(int propertyId, int userId, DateTime startDate, DateTime endDate, decimal totalPrice)
    {
        PropertyId = propertyId;
        UserId = userId;
        StartDate = startDate;
        EndDate = endDate;
        TotalPrice = totalPrice;
        CreatedAt = DateTime.UtcNow;
        Status = BookingStatus.Pending;
        PaymentStatus = "";
    }

    public void ConfirmBooking()
    {
        Status = BookingStatus.Confirmed;
    }

    public void CancelBooking()
    {
        Status = BookingStatus.Cancelled;
    }

    public void CompleteBooking()
    {
        Status = BookingStatus.Completed;
    }

    public void LinkPayment(int paymentId)
    {
        PaymentId = paymentId;
        PaymentStatus = "Linked";
    }

    public void UpdatePaymentStatus(string status)
    {
        PaymentStatus = status;

        if (status == "Succeeded")
        {
            Status = BookingStatus.Paid;
        }
    }
    public void RecordPaymentTransaction(string paymentMethod, string transactionId)
    {
        PaymentMethod = paymentMethod;
        TransactionId = transactionId;
        PaymentDate = DateTime.UtcNow;
    }

    public bool CanBeModified()
    {
        return Status == BookingStatus.Pending || Status == BookingStatus.Confirmed;
    }

    public void UpdateDates(DateTime startDate, DateTime endDate, decimal totalPrice)
    {
        if (!CanBeModified())
        {
            throw new InvalidOperationException($"Cannot modify booking in {Status} status");
        }

        StartDate = startDate;
        EndDate = endDate;
        TotalPrice = totalPrice;
    }
}

public static class BookingStatus
{
    public const string Pending = "Pending";
    public const string Confirmed = "Confirmed";
    public const string Paid = "Paid";
    public const string Cancelled = "Cancelled";
    public const string Completed = "Completed";

    public static readonly IReadOnlyList<string> AllStatuses = new[] { Pending, Confirmed, Paid, Cancelled, Completed };
}
