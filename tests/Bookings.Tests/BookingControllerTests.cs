using Bookings.Api.Application.Commands.Bookings;
using Bookings.Api.Application.Queries.Bookings;
using Bookings.Api.Controllers;
using Bookings.Api.Controllers.Request.Bookings;
using Bookings.Domain.AggregatesModel.BookingAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bookings.Tests
{
    [TestFixture]
    public class BookingControllerTests
    {
        private Mock<ILogger<BookingController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<IBookingQueries> _queriesMock;
        private BookingController _controller;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<BookingController>>();
            _mediatorMock = new Mock<IMediator>();
            _queriesMock = new Mock<IBookingQueries>();
            _controller = new BookingController(_loggerMock.Object, _mediatorMock.Object, _queriesMock.Object);
        }

        [Test]
        public async Task CreateBooking_WhenCommandSucceeds_ReturnsOkResult()
        {
            // Arrange
            var request = new CreateBookingRequest(
                PropertyId: 1,
                UserId: 2,
                StartDate: DateTime.Now.AddDays(1),
                EndDate: DateTime.Now.AddDays(3),
                CreatedAt: DateTime.Now,
                Status: BookingStatus.Pending,
                TotalPrice: 150.0m
            );

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateBooking(request);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(
                It.Is<CreateBookingCommand>(cmd =>
                    cmd.PropertyId == request.PropertyId &&
                    cmd.UserId == request.UserId &&
                    cmd.StartDate == request.StartDate &&
                    cmd.EndDate == request.EndDate &&
                    cmd.TotalPrice == request.TotalPrice),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task CreateBooking_WhenCommandFails_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new CreateBookingRequest(
                PropertyId: 1,
                UserId: 2,
                StartDate: DateTime.Now.AddDays(1),
                EndDate: DateTime.Now.AddDays(3),
                CreatedAt: DateTime.Now,
                Status: BookingStatus.Pending,
                TotalPrice: 150.0m
            );

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CreateBooking(request);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Failed to create booking.", badRequestResult.Value);
        }

        [Test]
        public async Task GetBookingById_WhenBookingExists_ReturnsOkResultWithBooking()
        {
            // Arrange
            int bookingId = 1;
            // Create a booking through the constructor instead of direct property assignment
            var booking = new Booking(
                propertyId: 1,
                userId: 2,
                startDate: DateTime.Now.AddDays(1),
                endDate: DateTime.Now.AddDays(3),
                totalPrice: 150.0m
            );

            // Use reflection to set the Id since it's a private setter
            typeof(Booking).GetProperty("Id").SetValue(booking, bookingId);

            _queriesMock
                .Setup(q => q.GetBookingByIdAsync(bookingId))
                .ReturnsAsync(booking);

            // Act
            var result = await _controller.GetBookingById(bookingId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(booking, okResult.Value);
        }

        [Test]
        public async Task GetBookingById_WhenBookingDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            int bookingId = 1;

            _queriesMock
                .Setup(q => q.GetBookingByIdAsync(bookingId))
                .ReturnsAsync((Booking)null);

            // Act
            var result = await _controller.GetBookingById(bookingId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GetUserBookings_WhenBookingsExist_ReturnsOkResultWithBookings()
        {
            // Arrange
            int userId = 1;
            var bookings = new List<Booking>
            {
                CreateTestBooking(1, 1, userId),
                CreateTestBooking(2, 2, userId)
            };

            _queriesMock
                .Setup(q => q.GetUserBookingsAsync(userId))
                .ReturnsAsync(bookings);

            // Act
            var result = await _controller.GetUserBookings(userId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(bookings, okResult.Value);
        }

        [Test]
        public async Task GetUserBookings_WhenUserHasNoBookings_ReturnsNotFoundResult()
        {
            // Arrange
            int userId = 1;

            _queriesMock
                .Setup(q => q.GetUserBookingsAsync(userId))
                .ReturnsAsync(new List<Booking>());

            // Act
            var result = await _controller.GetUserBookings(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task CancelBooking_WhenCommandSucceeds_ReturnsOkResult()
        {
            // Arrange
            int bookingId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CancelBookingCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CancelBooking(bookingId);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(
                It.Is<CancelBookingCommand>(cmd => cmd.BookingId == bookingId),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task CancelBooking_WhenCommandFails_ReturnsNotFoundResult()
        {
            // Arrange
            int bookingId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CancelBookingCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CancelBooking(bookingId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        // Helper method to create test bookings with proper initialization
        private Booking CreateTestBooking(int id, int propertyId, int userId)
        {
            var booking = new Booking(
                propertyId: propertyId,
                userId: userId,
                startDate: DateTime.Now.AddDays(1),
                endDate: DateTime.Now.AddDays(3),
                totalPrice: 150.0m
            );

            typeof(Booking).GetProperty("Id").SetValue(booking, id);

            return booking;
        }
    }
}