using Bookings.Domain.AggregatesModel.PropertyAggregate;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Bookings.Application.Queries.Properties;
public class PropertyQueries : IPropertyQueries
{
    private readonly string _connectionString;

    public PropertyQueries(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("RentFinder");
    }

    public async Task<Property> GetPropertyByIdAsync(int propertyId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Property>(
            "SELECT * FROM Properties WHERE Id = @PropertyId",
            new { PropertyId = propertyId }
        );
    }

    public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryAsync<Property>(
            "SELECT * FROM Properties"
        );
    }

    public async Task<IEnumerable<Property>> GetPropertiesByStatusAsync(string status)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryAsync<Property>(
            "SELECT * FROM Properties WHERE Status = @Status",
            new { Status = status }
        );
    }

    public async Task<IEnumerable<Property>> GetPropertiesByOwnerAsync(int ownerId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryAsync<Property>(
            "SELECT * FROM Properties WHERE OwnerId = @OwnerId",
            new { OwnerId = ownerId }
        );
    }

    public async Task<PropertyImage> GetPropertyImageAsync(int propertyId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<PropertyImage>(
           @"SELECT Id, PropertyId, ImageData, ImageType, Caption, UploadedAt 
              FROM propertyimage 
              WHERE PropertyId = @PropertyId 
              ORDER BY Id ASC 
              LIMIT 1",
           new { PropertyId = propertyId }
       );
    }
    public async Task<int> GetMostRecentPropertyIdByOwnerAsync(int ownerId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT Id FROM Properties WHERE OwnerId = @OwnerId ORDER BY CreatedAt DESC LIMIT 1",
            new { OwnerId = ownerId }
        );
    }
}
