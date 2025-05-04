namespace Bookings.Api.Controllers.Request.Users;

public record UpdateRoleRequest(
    string Auth0Id,
    string Role
    );

