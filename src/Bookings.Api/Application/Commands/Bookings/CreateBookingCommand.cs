using MediatR;

namespace Bookings.Api.Application.Commands.Bookings;

public record CreateBookingCommand(
    int PropertyId,
    int UserId,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalPrice
) : IRequest<bool>;

