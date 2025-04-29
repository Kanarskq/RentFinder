using Bookings.Api.Apis;
using Bookings.Api.Application.Commands.Bookings;
using Bookings.Api.Application.Commands.Identities;
using Bookings.Api.Application.Queries.Bookings;
using Bookings.Api.Infrastructure.Services.Bookings;
using Bookings.Api.Infrastructure.Services.Identities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.UnitTests;

[TestFixture]
public class BookingApiTests
{
    private Mock<IMediator> _mediatorMock;
    private Mock<IBookingQueries> _queriesMock;
    private Mock<IIdentityService> _identityServiceMock;
    private Mock<ILogger<BookingServices>> _loggerMock;
    private BookingServices _bookingServices;

    [SetUp]
    public void Setup()
    {
        _mediatorMock = new Mock<IMediator>();
        _queriesMock = new Mock<IBookingQueries>();
        _identityServiceMock = new Mock<IIdentityService>();
        _loggerMock = new Mock<ILogger<BookingServices>>();

        _bookingServices = new BookingServices(
            _mediatorMock.Object,
            _queriesMock.Object,
            _identityServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Test]
    public async Task CreateBookingAsync_WithValidRequest_ReturnsOkResult()
    {
        var requestId = Guid.NewGuid();
        var request = new CreateBookingRequest(
            BookingId: 1,
            PropertyId: 100,
            UserId: 200,
            StartDate: DateTime.UtcNow.AddDays(1),
            EndDate: DateTime.UtcNow.AddDays(5),
            CreatedAt: DateTime.UtcNow,
            Status: "Pending"
        );

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<IdentifiedCommand<CreateBookingCommand, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await BookingApi.CreateBookingAsync(requestId, request, _bookingServices);

        Assert.IsInstanceOf<Ok>(result.Result);
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<IdentifiedCommand<CreateBookingCommand, bool>>(cmd =>
                    cmd.Command.PropertyId == request.PropertyId &&
                    cmd.Command.UserId == request.UserId &&
                    cmd.Command.StartDate == request.StartDate &&
                    cmd.Command.EndDate == request.EndDate &&
                    cmd.Id == requestId
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Test]
    public async Task CreateBookingAsync_WithEmptyRequestId_ReturnsBadRequest()
    {
        var requestId = Guid.Empty;
        var request = new CreateBookingRequest(
            BookingId: 1,
            PropertyId: 100,
            UserId: 200,
            StartDate: DateTime.UtcNow.AddDays(1),
            EndDate: DateTime.UtcNow.AddDays(5),
            CreatedAt: DateTime.UtcNow,
            Status: "Pending"
        );

        var result = await BookingApi.CreateBookingAsync(requestId, request, _bookingServices);

        Assert.IsInstanceOf<BadRequest<string>>(result.Result);
        var badRequestResult = result.Result as BadRequest<string>;
        Assert.NotNull(badRequestResult);
        Assert.AreEqual("RequestId is missing.", badRequestResult.Value);
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<IdentifiedCommand<CreateBookingCommand, bool>>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task CreateBookingAsync_WhenCommandFails_ReturnsBadRequest()
    {
        var requestId = Guid.NewGuid();
        var request = new CreateBookingRequest(
            BookingId: 1,
            PropertyId: 100,
            UserId: 200,
            StartDate: DateTime.UtcNow.AddDays(1),
            EndDate: DateTime.UtcNow.AddDays(5),
            CreatedAt: DateTime.UtcNow,
            Status: "Pending"
        );

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<IdentifiedCommand<CreateBookingCommand, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await BookingApi.CreateBookingAsync(requestId, request, _bookingServices);

        Assert.IsInstanceOf<BadRequest<string>>(result.Result);
        var badRequestResult = result.Result as BadRequest<string>;
        Assert.NotNull(badRequestResult);
        Assert.AreEqual("Failed to create booking.", badRequestResult.Value);
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<IdentifiedCommand<CreateBookingCommand, bool>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task CreateBookingAsync_VerifyLogging_LogsAppropriateMessages()
    {
        var requestId = Guid.NewGuid();
        var request = new CreateBookingRequest(
            BookingId: 1,
            PropertyId: 100,
            UserId: 200,
            StartDate: DateTime.UtcNow.AddDays(1),
            EndDate: DateTime.UtcNow.AddDays(5),
            CreatedAt: DateTime.UtcNow,
            Status: "Pending"
        );

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<IdentifiedCommand<CreateBookingCommand, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _loggerMock.Setup(x => x.BeginScope(It.IsAny<List<KeyValuePair<string, object>>>()))
            .Returns(Mock.Of<IDisposable>());

        await BookingApi.CreateBookingAsync(requestId, request, _bookingServices);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received booking request for UserId")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Sending CreateBookingCommand for UserId")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CreateBookingCommand succeeded")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}