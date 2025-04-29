using Bookings.Domain.AggregatesModel.PaymentAggregate;
using Bookings.Infrastructure.Repositories;
using Bookings.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bookings.Infrastructure;

public static class PaymentServicesRegistration
{
    public static void AddPaymentServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register options
        //services.Configure<GooglePayOptions>(configuration.GetSection(GooglePayOptions.GooglePay));

        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddScoped<GooglePayService>();

        services.AddScoped<IPaymentServiceFactory, PaymentServiceFactory>();
    }
}
