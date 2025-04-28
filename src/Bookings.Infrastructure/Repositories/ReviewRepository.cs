using Bookings.Domain.AggregatesModel.ReviewAggregate;
using Bookings.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly BookingContext _context;

    public ReviewRepository(BookingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public Review Add(Review review)
    {
        return _context.Reviews.Add(review).Entity;
    }

    public async Task<Review?> GetAsync(int reviewId)
    {
        return await _context.Reviews
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reviewId);
    }

    public async Task<IEnumerable<Review>> GetByPropertyAsync(int propertyId)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => r.PropertyId == propertyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByUserAsync(int userId)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<double> GetAverageRatingForPropertyAsync(int propertyId)
    {
        return await _context.Reviews
            .Where(r => r.PropertyId == propertyId)
            .AverageAsync(r => r.Rating);
    }

    public void Update(Review review)
    {
        _context.Reviews.Update(review);
    }
}
