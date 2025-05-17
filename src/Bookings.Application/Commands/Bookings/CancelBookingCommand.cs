using MediatR;

namespace Bookings.Application.Commands.Bookings;

public record CancelBookingCommand(int BookingId) : IRequest<bool>;
