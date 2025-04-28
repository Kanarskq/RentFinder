using MediatR;

namespace Bookings.Api.Application.Commands.Properties;

public record DeletePropertyImageCommand(
    int PropertyId,
    int ImageId
) : IRequest<bool>;
