using Bookings.Domain.AggregatesModel.BookingAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bookings.Api.Application.Commands
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
                request.EndDate
            );

            _bookingRepository.Add(booking);

            _logger.LogInformation("Booking successfully created for UserId: {UserId}", request.UserId);

            return true;
        }
    }
}
