using Auth0.AspNetCore.Authentication;
using Authetication.Api.Data;
using Authetication.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Api.Controllers;

[ApiController]
[Route("auth/[action]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task Login(string returnUrl = "https://localhost:7105/")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri(returnUrl)
        .WithParameter("grant_type", "client_credentials")
        .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var idToken = await HttpContext.GetTokenAsync("id_token");
        // get roles from the token

        return Ok(new
        {
            Name = User.Identity.Name,
            EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
            ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
            Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value),
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
            .WithRedirectUri("https://localhost:7105/")
            .Build();

        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Name == request.Name || u.Email == request.Email))
        {
            return BadRequest("Username or email already exists");
        }

        if (!UserRoles.AllRoles.Contains(request.Role))
        {
            return BadRequest("Invalid role");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == request.Name);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid username or password");
        }

        var token = GenerateJwtToken(user);

        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("userId", user.UserId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record RegisterRequest(string Name, string Email, string Password, string Role);
public record LoginRequest(string Name, string Password);


