using Bookings.Domain.AggregatesModel.ReviewAggregate;
using Dapper;
using MySqlConnector;

namespace Bookings.Api.Application.Queries.Reviews;

public class ReviewQueries : IReviewQueries
{
    private readonly string _connectionString;

    public ReviewQueries(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("RentFinder");
    }

    public async Task<Review> GetReviewByIdAsync(int reviewId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Review>(
            "SELECT * FROM Reviews WHERE ReviewId = @ReviewId",
            new { ReviewId = reviewId }
        );
    }

    public async Task<IEnumerable<Review>> GetReviewsByPropertyAsync(int propertyId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryAsync<Review>(
            "SELECT * FROM Reviews WHERE PropertyId = @PropertyId",
            new { PropertyId = propertyId }
        );
    }

    public async Task<IEnumerable<Review>> GetReviewsByUserAsync(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryAsync<Review>(
            "SELECT * FROM Reviews WHERE UserId = @UserId",
            new { UserId = userId }
        );
    }

    public async Task<double> GetAveragePropertyRatingAsync(int propertyId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<double>(
            "SELECT AVG(Rating) FROM Reviews WHERE PropertyId = @PropertyId",
            new { PropertyId = propertyId }
        );
    }
}
