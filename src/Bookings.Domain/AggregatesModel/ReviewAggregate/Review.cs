using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.ReviewAggregate;

public class Review : Entity, IAggregateRoot
{
    public int Id { get; private set; }
    public int PropertyId { get; private set; }
    public int UserId { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Review() { }

    public Review(int propertyId, int userId, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");

        PropertyId = propertyId;
        UserId = userId;
        Rating = rating;
        Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateReview(int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");

        Rating = rating;
        Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        UpdatedAt = DateTime.UtcNow;
    }
}