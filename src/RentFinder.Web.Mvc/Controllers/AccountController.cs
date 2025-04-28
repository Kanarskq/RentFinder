using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RentFinder.Web.Mvc.Models;
using RentFinder.Web.Mvc.Services;
using Microsoft.AspNetCore.Authorization;

namespace RentFinder.Web.Mvc.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.LoginAsync(model);
        if (response != null && !string.IsNullOrEmpty(response.Token))
        {
            return Ok(response);
        }

        return Unauthorized(new { message = "Invalid login attempt" });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.RegisterAsync(model);
        if (response != null && !string.IsNullOrEmpty(response.Token))
        {
            return Ok(response);
        }

        return BadRequest(new { message = "Registration failed" });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var profile = await _authService.GetUserProfileAsync();
        if (profile == null)
        {
            return NotFound();
        }

        return Ok(profile);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(new { message = "Logged out successfully" });
    }
}