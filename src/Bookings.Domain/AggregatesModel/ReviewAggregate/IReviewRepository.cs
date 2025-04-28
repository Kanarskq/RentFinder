using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.ReviewAggregate;

public interface IReviewRepository : IRepository<Review>
{
    Review Add(Review review);
    void Update(Review review);
    Task<Review?> GetAsync(int reviewId);
    Task<IEnumerable<Review>> GetByPropertyAsync(int propertyId);
    Task<IEnumerable<Review>> GetByUserAsync(int userId);
    Task<double> GetAverageRatingForPropertyAsync(int propertyId);
}
