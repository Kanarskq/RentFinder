using Bookings.Domain.AggregatesModel.ReviewAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookings.Application.Commands.Reviews;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, bool>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<UpdateReviewCommandHandler> _logger;

    public UpdateReviewCommandHandler(
        IReviewRepository reviewRepository,
        ILogger<UpdateReviewCommandHandler> logger)
    {
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating review ReviewId: {ReviewId}, New Rating: {Rating}",
            request.ReviewId, request.Rating);

        try
        {
            var existingReview = await _reviewRepository.GetAsync(request.ReviewId);
            if (existingReview == null)
            {
                _logger.LogWarning("Review not found for ReviewId: {ReviewId}", request.ReviewId);
                return false;
            }

            existingReview.UpdateReview(request.Rating, request.Comment);

            _reviewRepository.Update(existingReview);

            var result = await _reviewRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            _logger.LogInformation(
                "Review updated successfully for ReviewId: {ReviewId}",
                request.ReviewId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error updating review for ReviewId: {ReviewId}",
                request.ReviewId);
            return false;
        }
    }
}
