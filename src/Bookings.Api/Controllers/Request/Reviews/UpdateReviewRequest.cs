namespace Bookings.Api.Controllers.Request.Reviews;

public record UpdateReviewRequest(
    int Rating,
    string Comment
);
