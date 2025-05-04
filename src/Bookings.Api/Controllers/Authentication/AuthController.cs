using Auth0.AspNetCore.Authentication;
using Bookings.Api.Controllers.Request.Users;
using Bookings.Api.Infrastructure.Services.Users;
using Bookings.Domain.AggregatesModel.BookingAggregate;
using Bookings.Domain.AggregatesModel.UserAggregate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Bookings.Api.Controllers.Authentication;

[ApiController]
[Route("auth/[action]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BookingController> _logger;

    public AuthController(IUserService userService, IConfiguration configuration, ILogger<BookingController> logger)
    {
        _userService = userService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public async Task Authorize(string returnUrl = "https://localhost:3000/auth/callback")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri(returnUrl)
        .WithParameter("grant_type", "client_credentials")
        .WithScope("openid profile email")
        .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var idToken = await HttpContext.GetTokenAsync("id_token");
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var user = auth0Id != null ? await _userService.GetUserByAuth0IdAsync(auth0Id) : null;
        _logger.LogInformation("Profile method called for user with Auth0 ID: {auth0Id}", auth0Id);
        _logger.LogInformation("Access token available: {hasToken}", !string.IsNullOrEmpty(accessToken));
        var result = new
        {
            Name = User.Identity.Name,
            EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
            ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
            Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value),
            Id = user?.Id,
            Role = user?.Role,
            AccessToken = accessToken,
            IdToken = idToken,
            CreatedAt = user?.CreatedAt,
            Phone = user?.PhoneNumber
        };

        _logger.LogInformation("Returning profile data for user: {email}", result.EmailAddress);

        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task Logout()
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be added to the
                // **Allowed Logout URLs** settings for the app.
                .WithRedirectUri("https://localhost:3000/")
                .Build();

        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [Authorize]
    [HttpPut("role")]
    public async Task<IActionResult> UpdateRole(UpdateRoleRequest request)
    {
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(auth0Id))
        {
            return Unauthorized("User not authenticated");
        }

        if (!User.IsInRole(UserRoles.Admin))
        {
            return Forbid("Only administrators can change user roles");
        }

        var result = await _userService.UpdateUserRoleAsync(request.Auth0Id, request.Role);
        if (!result)
        {
            return BadRequest("Invalid role or user not found");
        }

        return Ok(new { message = "Role updated successfully" });
    }
}


