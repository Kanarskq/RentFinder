namespace Bookings.Api.Application.Queries.Users;

public interface IUserQueries
{
    Task<string> GetUserNameById(int userId);
}
