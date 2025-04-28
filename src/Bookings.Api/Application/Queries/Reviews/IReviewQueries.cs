using Bookings.Domain.AggregatesModel.ReviewAggregate;

namespace Bookings.Api.Application.Queries.Reviews;

public interface IReviewQueries
{
    Task<Review> GetReviewByIdAsync(int reviewId);
    Task<IEnumerable<Review>> GetReviewsByPropertyAsync(int propertyId);
    Task<IEnumerable<Review>> GetReviewsByUserAsync(int userId);
    Task<double> GetAveragePropertyRatingAsync(int propertyId);
}