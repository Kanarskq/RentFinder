using MediatR;

namespace Bookings.Api.Application.Commands.Properties;

public record MarkPropertyAsAvailableCommand(int PropertyId) : IRequest<bool>;

