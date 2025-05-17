using MediatR;

namespace Bookings.Application.Commands.Properties;

public record MarkPropertyAsAvailableCommand(int PropertyId) : IRequest<bool>;

