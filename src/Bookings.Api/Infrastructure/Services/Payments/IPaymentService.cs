using Bookings.Api.Controllers.Request.Payments;
using Stripe;

namespace Bookings.Api.Infrastructure.Services.Payments;
public interface IPaymentService
{
     Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
}
