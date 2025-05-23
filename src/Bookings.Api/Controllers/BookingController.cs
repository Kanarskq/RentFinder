﻿using Bookings.Api.Controllers.Request.Bookings;
using Bookings.Application.Commands.Bookings;
using Bookings.Application.Queries.Bookings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookings.Api.Controllers;

[ApiController]
[Route("api/booking")]
public class BookingController : ControllerBase
{
    private readonly ILogger<BookingController> _logger;
    private readonly IMediator _mediator;
    private readonly IBookingQueries _queries;

    public BookingController(
        ILogger<BookingController> logger,
        IMediator mediator,
        IBookingQueries queries)
    {
        _logger = logger;
        _mediator = mediator;
        _queries = queries;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        _logger.LogInformation("Creating booking for UserId: {UserId}", request.UserId);

        var command = new CreateBookingCommand(
            request.PropertyId,
            request.UserId,
            request.StartDate,
            request.EndDate,
            request.TotalPrice
        );

        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok();
        }

        return BadRequest("Failed to create booking.");
    }

    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBookingById(int bookingId)
    {
        _logger.LogInformation("Getting booking with ID: {BookingId}", bookingId);

        var booking = await _queries.GetBookingByIdAsync(bookingId);

        if (booking == null)
        {
            return NotFound();
        }

        return Ok(booking);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserBookings(int userId)
    {
        _logger.LogInformation("Getting bookings for user with ID: {userId}", userId);

        var bookings = await _queries.GetUserBookingsAsync(userId);

        if (bookings == null || !bookings.Any())
        {
            return NotFound();
        }

        return Ok(bookings);
    }

    [HttpDelete("{bookingId}")]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        _logger.LogInformation("Cancelling booking with ID: {BookingId}", bookingId);

        var command = new CancelBookingCommand(bookingId);
        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok();
        }

        return NotFound();
    }
}