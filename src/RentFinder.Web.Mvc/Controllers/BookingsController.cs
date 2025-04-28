using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFinder.Web.Mvc.Models;
using RentFinder.Web.Mvc.Services;

namespace RentFinder.Web.Mvc.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserBookings()
    {
        var bookings = await _bookingService.GetUserBookingsAsync();
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookingCreateModel bookingModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _bookingService.CreateBookingAsync(bookingModel);
        if (result != null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.BookingId }, result);
        }

        return BadRequest("Failed to create booking");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _bookingService.GetBookingDetailAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        return Ok(booking);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _bookingService.CancelBookingAsync(id);
        if (result)
        {
            return Ok(new { success = true, message = "Booking cancelled successfully" });
        }

        return BadRequest(new { success = false, message = "Failed to cancel booking" });
    }
}