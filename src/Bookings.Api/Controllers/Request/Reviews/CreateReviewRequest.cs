namespace Bookings.Api.Controllers.Request.Reviews;

public record CreateReviewRequest(
    int PropertyId,
    int UserId,
    int Rating,
    string Comment
);
