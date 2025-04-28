using Bookings.Domain.AggregatesModel.UserAggregate;
using Bookings.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BookingContext _context;

    public UserRepository(BookingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public User Add(User user)
    {
        return _context.Users.Add(user).Entity;
    }

    public async Task<User?> GetAsync(int userId)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetByAuth0IdAsync(string auth0Id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Auth0Id == auth0Id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(string role)
    {
        return await _context.Users
            .Where(u => u.Role == role)
            .ToListAsync();
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }
}
