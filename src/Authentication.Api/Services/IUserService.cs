using Authetication.Api.Models;

namespace Authetication.Api.Services;

public interface IUserService
{
    Task<User?> GetUserByAuth0IdAsync(string auth0Id);
    Task<User> CreateOrUpdateUserAsync(User user);
    Task<bool> UpdateUserRoleAsync(string auth0Id, string role);
}
