using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authetication.Api.Controllers;

[ApiController]
[Route("api/pesourses")]
public class ResoursesController : ControllerBase
{
    [HttpGet("messages")]
    [Authorize("read:messages")]
    public IActionResult GetMessages()
    {
        return Ok(new[]
        {
                new { Id = 1, Text = "Hello, World!" },
                new { Id = 2, Text = "Goodbye, World!" }
            });
    }
}
