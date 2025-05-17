using MediatR;

namespace Bookings.Application.Commands.Properties;

public record AddPropertyImageCommand(
    int PropertyId,
    byte[] ImageData,
    string ImageType,
    string Caption
) : IRequest<bool>;
