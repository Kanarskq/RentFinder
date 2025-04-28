using Bookings.Domain.AggregatesModel.BookingAggregate;
using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.PropertyAggregate;

public interface IPropertyRepository : IRepository<Property>
{
    Property Add(Property property);
    void Update(Property property);
    Task<Property?> GetAsync(int propertyId);
    Task<Property?> GetByOwnerAsync(int ownerId);
    Task<IEnumerable<Property>> GetAllAsync();
    Task<IEnumerable<Property>> GetByStatusAsync(string status);
}
