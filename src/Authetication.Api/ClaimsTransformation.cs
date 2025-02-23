using Authetication.Api.Models;
using Authetication.Api.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Authetication.Api;

public class ClaimsTransformation : IClaimsTransformation
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;

    public ClaimsTransformation(
        IHttpContextAccessor httpContextAccessor,
        IUserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }

        var auth0Id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(auth0Id))
        {
            return principal;
        }

        var user = await _userService.GetUserByAuth0IdAsync(auth0Id);
        if (user == null)
        {
            user = await _userService.CreateOrUpdateUserAsync(new Models.User
            {
                Auth0Id = auth0Id,
                Email = principal.FindFirst(ClaimTypes.Email)?.Value ?? "",
                Name = principal.FindFirst(ClaimTypes.Name)?.Value ?? "",
                Role = UserRoles.Tenant, // Default role
                EmailVerified = bool.Parse(principal.FindFirst("email_verified")?.Value ?? "false"),
            });
        }

        var identity = principal.Identity as ClaimsIdentity;
        if (!principal.HasClaim(c => c.Type == ClaimTypes.Role))
        {
            identity?.AddClaim(new Claim(ClaimTypes.Role, user.Role));
        }

        return principal;
    }
}
