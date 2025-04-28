using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.PaymentAggregate;

public class Payment : Entity, IAggregateRoot
{
    public int Id { get; private set; }
    public int BookingId { get; private set; }
    public int UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public string PaymentMethod { get; private set; }
    public string Status { get; private set; }
    public string? TransactionId { get; private set; }
    public string? ProviderReference { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? PaymentIntent { get; private set; }

    private Payment() { }

    public Payment(int bookingId, int userId, decimal amount, string currency, string paymentMethod)
    {
        BookingId = bookingId;
        UserId = userId;
        Amount = amount;
        Currency = currency;
        PaymentMethod = paymentMethod;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetAsInitiated(string paymentIntent)
    {
        Status = PaymentStatus.Initiated;
        PaymentIntent = paymentIntent;
    }

    public void SetAsSucceeded(string transactionId, string providerReference)
    {
        Status = PaymentStatus.Succeeded;
        TransactionId = transactionId;
        ProviderReference = providerReference;
        ProcessedAt = DateTime.UtcNow;
    }

    public void SetAsFailed(string errorMessage)
    {
        Status = PaymentStatus.Failed;
        ErrorMessage = errorMessage;
        ProcessedAt = DateTime.UtcNow;
    }

    public void SetAsCancelled(string reason)
    {
        Status = PaymentStatus.Cancelled;
        ErrorMessage = reason;
        ProcessedAt = DateTime.UtcNow;
    }

    public void SetAsRefunded(string refundReference)
    {
        Status = PaymentStatus.Refunded;
        ProviderReference = refundReference;
        ProcessedAt = DateTime.UtcNow;
    }

    public bool CanBeProcessed()
    {
        return Status == PaymentStatus.Pending || Status == PaymentStatus.Initiated;
    }

    public bool CanBeRefunded()
    {
        return Status == PaymentStatus.Succeeded;
    }
}
