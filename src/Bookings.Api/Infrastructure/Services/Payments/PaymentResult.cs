namespace Bookings.Api.Infrastructure.Services.Payments;
public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public string ErrorMessage { get; set; }
}
