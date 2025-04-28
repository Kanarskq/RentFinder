using Bookings.Domain.AggregatesModel.PaymentAggregate;
using Bookings.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bookings.Infrastructure.Services;

public class PaymentServiceFactory : IPaymentServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPaymentService CreatePaymentService(string paymentMethod)
    {
        if (!PaymentMethod.IsValid(paymentMethod))
        {
            throw new ArgumentException($"Invalid payment method: {paymentMethod}");
        }

        return paymentMethod switch
        {
            PaymentMethod.GooglePay => _serviceProvider.GetRequiredService<GooglePayService>(),
            _ => throw new NotImplementedException($"Payment method {paymentMethod} is not implemented yet")
        };
    }
}