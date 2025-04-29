using Bookings.Domain.AggregatesModel.PropertyAggregate;

namespace Bookings.Api.Application.Queries.Properties;

public interface IPropertyQueries
{
    Task<Property> GetPropertyByIdAsync(int propertyId);
    Task<IEnumerable<Property>> GetAllPropertiesAsync();
    Task<IEnumerable<Property>> GetPropertiesByStatusAsync(string status);
    Task<IEnumerable<Property>> GetPropertiesByOwnerAsync(int ownerId);
    Task<PropertyImage> GetPropertyImageAsync(int propertyId);
    Task<int> GetMostRecentPropertyIdByOwnerAsync(int ownerId);
}
