using Bookings.Domain.AggregatesModel.PropertyAggregate;
using Bookings.Domain.AggregatesModel.ReviewAggregate;
using Bookings.Domain.AggregatesModel.UserAggregate;
using Bookings.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bookings.Infrastructure;

public static class RentFinderRegistration
{
    private const string ConnectionStringName = "RentFinder";

    public static void AddRentFinderPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
                               ?? throw new InvalidOperationException($"Connection string: '{ConnectionStringName}' is not found in configurations.");

        services.AddDbContext<BookingContext>(options =>
        {
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mysqlOptions =>
                {
                    mysqlOptions.MigrationsHistoryTable(
                        BookingContext.RentFinderDbMigrationsHistoryTable);
                });
        });

        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
    }
}
