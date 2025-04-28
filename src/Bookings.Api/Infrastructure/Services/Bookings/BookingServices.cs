using Bookings.Api.Application.Queries.Bookings;
using MediatR;

namespace Bookings.Api.Infrastructure.Services.Bookings;

public class BookingServices(
    IMediator mediator,
    IBookingQueries queries,
    ILogger<BookingServices> logger)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<BookingServices> Logger { get; } = logger;
    public IBookingQueries Queries { get; } = queries;
}
