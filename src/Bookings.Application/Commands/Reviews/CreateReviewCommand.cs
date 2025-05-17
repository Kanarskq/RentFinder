using MediatR;

namespace Bookings.Application.Commands.Reviews;

public record CreateReviewCommand(
    int PropertyId,
    int UserId,
    int Rating,
    string Comment) : IRequest<bool>;
