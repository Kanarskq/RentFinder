using Bookings.Application.Queries.Bookings;
using Bookings.Application.Queries.Properties;
using Bookings.Application.Queries.Reviews;
using Bookings.Application.Queries.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bookings.Application;

public static class ApplicationRegistration
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddScoped<IBookingQueries, BookingQueries>();
        services.AddScoped<IPropertyQueries, PropertyQueries>();
        services.AddScoped<IReviewQueries, ReviewQueries>();
        services.AddScoped<IUserQueries, UserQueries>();
    }
}
