using Bookings.Domain.AggregatesModel.ReviewAggregate;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Bookings.Application.Queries.Users;

public class UserQueries : IUserQueries
{
    private readonly string _connectionString;

    public UserQueries(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("RentFinder");
    }
    public async Task<string> GetUserNameById(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var name = await connection.QueryFirstOrDefaultAsync<string>(
            "SELECT Name FROM users WHERE Id = @userId",
            new { userId }
        );

        return name ?? "Anonymous";
    }

    public async Task<int> GetUserIdByEmail(string email)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var id = await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT Id FROM users WHERE Email = @email",
            new { email }
        );

        return id;
    }
}
