using Bookings.Application.Queries.Bookings;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookings.Application.Services.Bookings;

public class BookingServices(
    IMediator mediator,
    IBookingQueries queries,
    ILogger<BookingServices> logger)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<BookingServices> Logger { get; } = logger;
    public IBookingQueries Queries { get; } = queries;
}
