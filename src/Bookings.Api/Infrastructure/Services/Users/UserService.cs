using Bookings.Domain.AggregatesModel.UserAggregate;
using Bookings.Domain.SeedWork;

namespace Bookings.Api.Infrastructure.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = userRepository.UnitOfWork;
    }

    public async Task<User?> GetUserByAuth0IdAsync(string auth0Id)
    {
        var user = await _userRepository.GetByAuth0IdAsync(auth0Id);
        return user;
    }

    public async Task<User> CreateOrUpdateUserAsync(User user)
    {
        var existingUser = await _userRepository.GetByAuth0IdAsync(user.Auth0Id);

        if (existingUser == null)
        {
            // Create new user
            user.CreatedAt = DateTime.UtcNow;

            // Set default role if not specified
            if (string.IsNullOrEmpty(user.Role))
            {
                user.Role = UserRoles.Tenant;
            }

            _userRepository.Add(user);
            await _unitOfWork.SaveEntitiesAsync();

            return user;
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

            _userRepository.Update(existingUser);
            await _unitOfWork.SaveEntitiesAsync();

            return user;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<bool> UpdateUserRoleAsync(string auth0Id, string role)
    {
        if (!UserRoles.AllRoles.Contains(role))
        {
            return false;
        }

        var user = await _userRepository.GetByAuth0IdAsync(auth0Id);
        if (user == null)
        {
            return false;
        }

        user.Role = role;
        _userRepository.Update(user);
        return await _unitOfWork.SaveEntitiesAsync();
    }
}