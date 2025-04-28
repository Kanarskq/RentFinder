using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookings.Api.Controllers.Authentication;

[ApiController]
[Route("api/resourses")]
public class ResoursesController : ControllerBase
{
    [HttpGet("messages")]
    [Authorize("Admin")]
    public IActionResult GetMessages()
    {
        return Ok(new[]
        {
                new { Id = 1, Text = "Hello, World!" },
                new { Id = 2, Text = "Goodbye, World!" }
            });
    }
}
