using Bookings.Domain.AggregatesModel.ReviewAggregate;
using MediatR;

namespace Bookings.Api.Application.Commands.Reviews;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, bool>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<CreateReviewCommandHandler> _logger;

    public CreateReviewCommandHandler(
        IReviewRepository reviewRepository,
        ILogger<CreateReviewCommandHandler> logger)
    {
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating review for PropertyId: {PropertyId}, UserId: {UserId}, Rating: {Rating}",
            request.PropertyId, request.UserId, request.Rating);

        try
        {
            var review = new Review(
                request.PropertyId,
                request.UserId,
                request.Rating,
                request.Comment
            );

            _reviewRepository.Add(review);

            var result = await _reviewRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            _logger.LogInformation(
                "Review created successfully for PropertyId: {PropertyId}, UserId: {UserId}",
                request.PropertyId, request.UserId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error creating review for PropertyId: {PropertyId}, UserId: {UserId}",
                request.PropertyId, request.UserId);
            return false;
        }
    }
}
