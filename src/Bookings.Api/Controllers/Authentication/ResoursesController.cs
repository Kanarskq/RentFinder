using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookings.Api.Controllers.Authentication;

[ApiController]
[Route("/")]
public class ResoursesController : ControllerBase
{
    [HttpGet("")]
    [Authorize]
    public IActionResult GetMessages()
    {
        return Ok(new[]
        {
                new { Id = 1, Text = "Hello, World!" },
                new { Id = 2, Text = "Goodbye, World!" }
            });
    }
}
