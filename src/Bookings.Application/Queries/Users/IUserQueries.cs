namespace Bookings.Application.Queries.Users;

public interface IUserQueries
{
    Task<string> GetUserNameById(int userId);
    Task<int> GetUserIdByEmail(string email);
}
