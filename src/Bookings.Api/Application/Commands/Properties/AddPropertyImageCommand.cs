using MediatR;

namespace Bookings.Api.Application.Commands.Properties;

public record AddPropertyImageCommand(
    int PropertyId,
    byte[] ImageData,
    string ImageType,
    string Caption
) : IRequest<bool>;
