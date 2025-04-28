using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.PaymentAggregate;

public interface IPaymentRepository : IRepository<Payment>
{
    Payment Add(Payment payment);
    void Update(Payment payment);
    Task<Payment?> GetAsync(int paymentId);
    Task<Payment?> GetByTransactionIdAsync(string transactionId);
    Task<IEnumerable<Payment>> GetByBookingAsync(int bookingId);
    Task<IEnumerable<Payment>> GetByUserAsync(int userId);
    Task<IEnumerable<Payment>> GetByStatusAsync(string status);
}
