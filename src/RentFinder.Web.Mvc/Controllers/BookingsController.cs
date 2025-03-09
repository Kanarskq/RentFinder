using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFinder.Web.Mvc.Models;
using RentFinder.Web.Mvc.Services;

namespace RentFinder.Web.Mvc.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public async Task<IActionResult> Index()
    {
        var bookings = await _bookingService.GetUserBookingsAsync();
        return View(bookings);
    }

    public IActionResult Create(int propertyId)
    {
        return View(new BookingCreateModel { PropertyId = propertyId });
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookingCreateModel bookingModel)
    {
        if (!ModelState.IsValid)
        {
            return View(bookingModel);
        }

        var result = await _bookingService.CreateBookingAsync(bookingModel);
        if (result != null)
        {
            TempData["SuccessMessage"] = "Booking created successfully";
            return RedirectToAction(nameof(Details), new { id = result.BookingId });
        }

        ModelState.AddModelError("", "Failed to create booking");
        return View(bookingModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var booking = await _bookingService.GetBookingDetailAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _bookingService.CancelBookingAsync(id);
        if (result)
        {
            TempData["SuccessMessage"] = "Booking cancelled successfully";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "Failed to cancel booking";
        return RedirectToAction(nameof(Details), new { id });
    }
}
