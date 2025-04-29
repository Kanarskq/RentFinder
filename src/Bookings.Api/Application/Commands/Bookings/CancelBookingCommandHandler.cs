using Bookings.Domain.AggregatesModel.BookingAggregate;
using MediatR;

namespace Bookings.Api.Application.Commands.Bookings;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<CancelBookingCommandHandler> _logger;

    public CancelBookingCommandHandler(IBookingRepository bookingRepository, ILogger<CancelBookingCommandHandler> logger)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CancelBookingCommand for BookingId: {BookingId}", request.BookingId);

        var booking = await _bookingRepository.GetAsync(request.BookingId);

        if (booking == null)
        {
            _logger.LogWarning("Booking not found - BookingId: {BookingId}", request.BookingId);
            return false;
        }

        booking.CancelBooking();

        await _bookingRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("Booking successfully canceled - BookingId: {BookingId}", request.BookingId);

        return true;
    }
}
