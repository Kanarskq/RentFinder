using Bookings.Domain.AggregatesModel.PaymentAggregate;

namespace Bookings.Domain.Services;

public interface IPaymentService
{
    string PaymentMethod { get; }
    Task<PaymentResult> InitiatePaymentAsync(PaymentRequest request);
    Task<PaymentResult> ProcessPaymentAsync(string paymentIntent, string paymentToken);
    Task<PaymentResult> RefundPaymentAsync(string transactionId, decimal amount);
}