namespace Bookings.Api.Controllers.Request.Payments;

public class PaymentConfirmationRequest
{
    public string PaymentIntentId { get; set; }
    public int BookingId { get; set; }
}