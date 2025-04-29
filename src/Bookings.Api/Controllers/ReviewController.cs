using Bookings.Api.Application.Commands.Reviews;
using Bookings.Api.Application.Queries.Reviews;
using Bookings.Api.Application.Queries.Users;
using Bookings.Api.Controllers.Request.Reviews;
using Bookings.Domain.AggregatesModel.ReviewAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookings.Api.Controllers;

[ApiController]
[Route("api/review")]
public class ReviewController : ControllerBase
{
    private readonly ILogger<ReviewController> _logger;
    private readonly IMediator _mediator;
    private readonly IReviewQueries _queries;
    private readonly IUserQueries _userQueries;

    public ReviewController(
        ILogger<ReviewController> logger,
        IMediator mediator,
        IReviewQueries queries,
        IUserQueries userQueries)
    {
        _logger = logger;
        _mediator = mediator;
        _queries = queries;
        _userQueries = userQueries;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        _logger.LogInformation(
            "Creating review for PropertyId: {PropertyId} from UserId: {UserId}",
            request.PropertyId, request.UserId);

        var command = new CreateReviewCommand(
            request.PropertyId,
            request.UserId,
            request.Rating,
            request.Comment
        );

        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok();
        }

        return BadRequest("Failed to create review.");
    }

    [HttpGet("{reviewId}")]
    public async Task<IActionResult> GetReviewById(int reviewId)
    {
        _logger.LogInformation("Getting review with ID: {ReviewId}", reviewId);

        var review = await _queries.GetReviewByIdAsync(reviewId);

        if (review == null)
        {
            return NotFound();
        }

        return Ok(review);
    }

    [HttpGet("property/{propertyId}")]
    public async Task<IActionResult> GetReviewsByProperty(int propertyId)
    {
        _logger.LogInformation("Getting reviews for property with ID: {PropertyId}", propertyId);

        var reviews = await _queries.GetReviewsByPropertyAsync(propertyId);

        if (reviews == null || !reviews.Any())
        {
            return NotFound();
        }

        var reviewDtos = new List<object>();

        foreach (var review in reviews)
        {
            var userName = await _userQueries.GetUserNameById(review.UserId);

            reviewDtos.Add(new
            {
                review.Id,
                review.PropertyId,
                review.UserId,
                review.Rating,
                review.Comment,
                review.CreatedAt,
                review.UpdatedAt,
                UserName = userName
            });
        }

        return Ok(reviewDtos);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetReviewsByUser(int userId)
    {
        _logger.LogInformation("Getting reviews by user with ID: {UserId}", userId);

        var reviews = await _queries.GetReviewsByUserAsync(userId);

        if (reviews == null || !reviews.Any())
        {
            return NotFound();
        }

        return Ok(reviews);
    }

    [HttpGet("property/{propertyId}/rating")]
    public async Task<IActionResult> GetAveragePropertyRating(int propertyId)
    {
        _logger.LogInformation("Getting average rating for property with ID: {PropertyId}", propertyId);

        try
        {
            var averageRating = await _queries.GetAveragePropertyRatingAsync(propertyId);
            return Ok(averageRating);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get average rating");
            return NotFound();
        }
    }

    [HttpPut("{reviewId}")]
    public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewRequest request)
    {
        _logger.LogInformation("Updating review with ID: {ReviewId}", reviewId);

        var command = new UpdateReviewCommand(
            reviewId,
            request.Rating,
            request.Comment
        );

        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok();
        }

        return NotFound();
    }
}