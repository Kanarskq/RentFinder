using MediatR;

namespace Bookings.Api.Application.Commands.Reviews;

public record UpdateReviewCommand(
    int ReviewId, 
    int Rating, 
    string Comment) : IRequest<bool>;
