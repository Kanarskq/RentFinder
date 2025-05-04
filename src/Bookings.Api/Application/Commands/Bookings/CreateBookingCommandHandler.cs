using Bookings.Domain.AggregatesModel.BookingAggregate;
using MediatR;

namespace Bookings.Api.Application.Commands.Bookings
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<CreateBookingCommandHandler> _logger;

        public CreateBookingCommandHandler(IBookingRepository bookingRepository, ILogger<CreateBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateBookingCommand for UserId: {UserId} with GooglePay", request.UserId);

            var booking = new Booking(
                request.PropertyId,
                request.UserId,
                request.StartDate,
                request.EndDate,
                request.TotalPrice
            );

            _bookingRepository.Add(booking);

            var result = await _bookingRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (result)
            {
                _logger.LogInformation("Booking successfully created for UserId: {UserId}", request.UserId);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to save booking for UserId: {UserId}", request.UserId);
                return false;
            }
        }
    }
}
