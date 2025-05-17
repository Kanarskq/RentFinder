using MediatR;

namespace Bookings.Application.Commands.Properties;

public record MarkPropertyAsUnavailableCommand(int PropertyId) : IRequest<bool>;