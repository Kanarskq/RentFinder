using Authetication.Api.Data;
using Authetication.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Authetication.Api.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByAuth0IdAsync(string auth0Id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Auth0Id == auth0Id);
    }

    public async Task<User> CreateOrUpdateUserAsync(User user)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Auth0Id == user.Auth0Id);

        if (existingUser == null)
        {
            // New user
            _context.Users.Add(user);
        }
        else
        {
            // Update existing user
            existingUser.Email = user.Email;
            existingUser.Name = user.Name;
            existingUser.EmailVerified = user.EmailVerified;
            existingUser.LastLogin = DateTime.UtcNow;

            // Only update role if it's not already set
            if (string.IsNullOrEmpty(existingUser.Role))
            {
                existingUser.Role = UserRoles.Tenant; // Default role for new users
            }
        }

        await _context.SaveChangesAsync();
        return existingUser ?? user;
    }

    public async Task<bool> UpdateUserRoleAsync(string auth0Id, string role)
    {
        if (!UserRoles.AllRoles.Contains(role))
        {
            return false;
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Auth0Id == auth0Id);
        if (user == null)
        {
            return false;
        }

        user.Role = role;
        await _context.SaveChangesAsync();
        return true;
    }
}