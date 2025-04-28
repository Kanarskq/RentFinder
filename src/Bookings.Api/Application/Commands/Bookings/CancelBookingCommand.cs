using MediatR;

namespace Bookings.Api.Application.Commands.Bookings;

public record CancelBookingCommand(int BookingId) : IRequest<bool>;
