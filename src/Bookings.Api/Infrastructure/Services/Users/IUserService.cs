using Bookings.Domain.AggregatesModel.UserAggregate;

namespace Bookings.Api.Infrastructure.Services.Users;

public interface IUserService
{
    Task<User?> GetUserAsync(int userId);
    Task<User?> GetUserByAuth0IdAsync(string auth0Id);
    Task<User> CreateOrUpdateUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> UpdateUserRoleAsync(string auth0Id, string role);
}
