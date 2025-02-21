using Dapper;
using MySql.Data.MySqlClient;

namespace Bookings.Api.Application.Queries;

public class BookingQueries : IBookingQueries
{
    private readonly string _connectionString;

    public BookingQueries(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<Booking> GetBookingByIdAsync(int bookingId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Booking>(
            "SELECT * FROM Bookings WHERE BookingId = @BookingId",
            new { BookingId = bookingId }
        );
    }

    public async Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryAsync<Booking>(
            "SELECT * FROM Bookings WHERE UserId = @UserId",
            new { UserId = userId }
        );
    }
}
