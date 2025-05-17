namespace Bookings.Application.Services.Payments;

public class PaymentRequest
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "UAH";
    public string Description { get; set; } = "";
    public string PaymentMethod { get; set; } = "";
    public string PaymentToken { get; set; } = "";
    public string? PaymentId { get; set; }
    public string RenterId { get; set; } = "";
    public string LandlordId { get; set; } = "";
}