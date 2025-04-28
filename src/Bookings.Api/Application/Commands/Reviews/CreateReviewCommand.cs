using MediatR;

namespace Bookings.Api.Application.Commands.Reviews;

public record CreateReviewCommand(
    int PropertyId, 
    int UserId, 
    int Rating, 
    string Comment) : IRequest<bool>;
