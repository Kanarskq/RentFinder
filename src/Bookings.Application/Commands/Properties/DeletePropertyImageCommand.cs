using MediatR;

namespace Bookings.Application.Commands.Properties;

public record DeletePropertyImageCommand(
    int PropertyId,
    int ImageId
) : IRequest<bool>;
