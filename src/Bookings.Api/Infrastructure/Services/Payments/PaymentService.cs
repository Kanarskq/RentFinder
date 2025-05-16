using Bookings.Api.Controllers.Request.Payments;
using Bookings.Domain.AggregatesModel.PaymentAggregate;
using Stripe;

namespace Bookings.Api.Infrastructure.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly StripeClient _stripeClient;
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IConfiguration configuration, IPaymentRepository paymentRepository)
    {
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        _stripeClient = new StripeClient(StripeConfiguration.ApiKey);
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100),
                Currency = request.Currency.ToLower(),
                PaymentMethodTypes = new List<string> { "card" },
                Description = request.Description,
                Metadata = new Dictionary<string, string>
                {
                    { "renter_id", request.RenterId },
                    { "landlord_id", request.LandlordId }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            var paymentId = await InitiatePaymentRecord(
                request.BookingId,
                request.UserId,
                request.Amount,
                request.Currency,
                request.PaymentMethod,
                paymentIntent.Id);

            var confirmOptions = new PaymentIntentConfirmOptions
            {
                PaymentMethod = request.PaymentToken
            };

            var confirmedIntent = await service.ConfirmAsync(
                paymentIntent.Id,
                confirmOptions);

            if (confirmedIntent.Status == "succeeded")
            {
                await UpdatePaymentAsSucceeded(paymentId, confirmedIntent.Id, confirmedIntent.Id);

                return new PaymentResult
                {
                    Success = true,
                    TransactionId = confirmedIntent.Id
                };
            }

            await UpdatePaymentAsFailed(paymentId, $"Payment status: {confirmedIntent.Status}");

            return new PaymentResult
            {
                Success = false,
                ErrorMessage = $"Payment status: {confirmedIntent.Status}"
            };
        }
        catch (StripeException ex)
        {
            if (!string.IsNullOrEmpty(request.PaymentId))
            {
                await UpdatePaymentAsFailed(int.Parse(request.PaymentId), ex.StripeError.Message);
            }

            return new PaymentResult
            {
                Success = false,
                ErrorMessage = ex.StripeError.Message
            };
        }
    }

    private async Task<int> InitiatePaymentRecord(int bookingId, int userId, decimal amount, string currency, string paymentMethod, string paymentIntent)
    {
        var payment = new Payment(bookingId, userId, amount, currency, paymentMethod);
        payment.SetAsInitiated(paymentIntent);

        var savedPayment = _paymentRepository.Add(payment);
        await _paymentRepository.UnitOfWork.SaveEntitiesAsync();

        return savedPayment.Id;
    }

    private async Task UpdatePaymentAsSucceeded(int paymentId, string transactionId, string providerReference)
    {
        var payment = await _paymentRepository.GetAsync(paymentId);
        if (payment != null)
        {
            payment.SetAsSucceeded(transactionId, providerReference);
            _paymentRepository.Update(payment);
            await _paymentRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }

    private async Task UpdatePaymentAsFailed(int paymentId, string errorMessage)
    {
        var payment = await _paymentRepository.GetAsync(paymentId);
        if (payment != null)
        {
            payment.SetAsFailed(errorMessage);
            _paymentRepository.Update(payment);
            await _paymentRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}