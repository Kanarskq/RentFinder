using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.UserAggregate;

public interface IUserRepository : IRepository<User>
{
    User Add(User user);
    void Update(User user);
    Task<User?> GetAsync(int userId);
    Task<User?> GetByAuth0IdAsync(string auth0Id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetByRoleAsync(string role);
}
