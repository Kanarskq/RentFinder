using Auth0.AspNetCore.Authentication;
using Bookings.Api.Infrastructure.Services.Users;
using Bookings.Domain.AggregatesModel.UserAggregate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookings.Api.Controllers.Authentication;

[ApiController]
[Route("auth/[action]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task Authorize(string returnUrl = "https://localhost:7000/")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri(returnUrl)
        .WithParameter("grant_type", "client_credentials")
        .WithScope("openid profile email")
        .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }

    [HttpGet]
    [Route("/callback")]
    public async Task<IActionResult> Callback()
    {
        string frontendUrl = _configuration["AllowedOrigins:ReactApp"];

        string returnUrl = $"{frontendUrl}/auth/callback";

        // Add access token to the return URL if available
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        if (!string.IsNullOrEmpty(accessToken))
        {
            returnUrl = AddQueryParam(returnUrl, "access_token", accessToken);
        }

        return Redirect(returnUrl);
    }

    private string AddQueryParam(string url, string name, string value)
    {
        var uriBuilder = new UriBuilder(url);
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query[name] = value;
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var idToken = await HttpContext.GetTokenAsync("id_token");
        var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var user = auth0Id != null ? await _userService.GetUserByAuth0IdAsync(auth0Id) : null;

        return Ok(new
        {
            User.Identity.Name,
            EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
            ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
            Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value),
            user?.Id,
            user?.Role,
            AccessToken = accessToken,
            IdToken = idToken
        });
    }

    [Authorize]
    [HttpGet]
    public async Task Logout()
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be added to the
                // **Allowed Logout URLs** settings for the app.
                .WithRedirectUri("https://localhost:7000/")
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

        // Only admins can change roles
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

public record UpdateRoleRequest(string Auth0Id, string Role);


