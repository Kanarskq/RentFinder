using MediatR;

namespace Bookings.Api.Application.Commands;

public record CreateBookingCommand(
    int PropertyId,
    int UserId,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<bool>;

